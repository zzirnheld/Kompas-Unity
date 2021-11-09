using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.GameCore
{
    public class ServerBoardController : BoardController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifierByIndex(int index) => ServerGame.ServerPlayers[index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        public override void Play(GameCard toPlay, Space to, Player controller, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: toPlay, stackable: stackSrc, triggerer: controller, space: to);
            bool wasKnown = toPlay.KnownToEnemy;
            base.Play(toPlay, to, controller);
            EffectsController.TriggerForCondition(Trigger.Play, context);
            EffectsController.TriggerForCondition(Trigger.Arrive, context);
            if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, to, wasKnown);
        }

        public override void Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            //calculate distance before doing the swap
            int distance = card.DistanceTo(to);
            var at = GetCardAt(to);
            //save info before for triggers
            var atInfo = at == null ? null : new GameCardInfo(at);
            var atAugs = new List<GameCardInfo>();
            if (at != null) atAugs.AddRange(at.Augments.Select(a => new GameCardInfo(a)));
            var cardInfo = new GameCardInfo(card);
            var cardAugs = new List<GameCardInfo>(card.Augments.Select(a => new GameCardInfo(a)));

            //actually perform the swap
            base.Swap(card, to, playerInitiated);

            //then trigger appropriate triggers. list of contexts:
            List<ActivationContext> ctxts = new List<ActivationContext>();
            //trigger for first card
            ctxts.Add(new ActivationContext(card: cardInfo, stackable: stackSrc, space: to,
                triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));
            //trigger for first card's augments
            foreach (var aug in cardAugs)
            {
                ctxts.Add(new ActivationContext(card: aug, stackable: null, space: to,
                    triggerer: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
            }

            if (at != null)
            {
                //then trigger this card's triggers
                ctxts.Add(new ActivationContext(card: atInfo, stackable: stackSrc, space: to,
                    triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));

                //trigger for first card's augments
                foreach (var aug in atAugs)
                {
                    ctxts.Add( new ActivationContext(card: aug, stackable: null, space: to,
                        triggerer: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
                }
            }

            EffectsController.TriggerForCondition(Trigger.Move, ctxts.ToArray());
            EffectsController.TriggerForCondition(Trigger.Arrive, ctxts.ToArray());

            //notify the players
            ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, to);
        }
    }
}