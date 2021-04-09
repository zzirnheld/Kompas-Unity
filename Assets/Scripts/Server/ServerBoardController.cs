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

        public override bool Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: toPlay, stackable: stackSrc, triggerer: controller, space: (toX, toY));
            bool wasKnown = toPlay.KnownToEnemy;
            if (base.Play(toPlay, toX, toY, controller))
            {
                EffectsController.TriggerForCondition(Trigger.Play, context);
                EffectsController.TriggerForCondition(Trigger.Arrive, context);
                if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, toX, toY, wasKnown);
                return true;
            }
            return false;
        }

        public override bool Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
        {
            if (card.IsAvatar && !card.Summoned) return false;
            if (card.Location != CardLocation.Field) return false;

            //calculate distance before doing the swap
            int distance = card.DistanceTo(toX, toY);
            var at = GetCardAt(toX, toY);
            //save info before for triggers
            var atInfo = at == null ? null : new GameCardInfo(at);
            var atAugs = new List<GameCardInfo>();
            if (at != null) atAugs.AddRange(at.Augments.Select(a => new GameCardInfo(a)));
            var cardInfo = new GameCardInfo(card);
            var cardAugs = new List<GameCardInfo>(card.Augments.Select(a => new GameCardInfo(a)));


            if (base.Swap(card, toX, toY, playerInitiated))
            {
                //then trigger appropriate triggers. list of contexts:
                List<ActivationContext> ctxts = new List<ActivationContext>();
                //trigger for first card
                ctxts.Add(new ActivationContext(card: cardInfo, stackable: stackSrc, space: (toX, toY),
                    triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));
                //trigger for first card's augments
                foreach (var aug in cardAugs)
                {
                    ctxts.Add(new ActivationContext(card: aug, stackable: null, space: (toX, toY),
                        triggerer: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
                }

                if (at != null)
                {
                    //then trigger this card's triggers
                    ctxts.Add(new ActivationContext(card: atInfo, stackable: stackSrc, space: (toX, toY),
                        triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));

                    //trigger for first card's augments
                    foreach (var aug in atAugs)
                    {
                        ctxts.Add( new ActivationContext(card: aug, stackable: null, space: (toX, toY),
                            triggerer: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
                    }
                }

                EffectsController.TriggerForCondition(Trigger.Move, ctxts.ToArray());
                EffectsController.TriggerForCondition(Trigger.Arrive, ctxts.ToArray());

                //notify the players
                ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, toX, toY);
                return true;
            }
            return false;
        }

        public override bool RemoveFromBoard(GameCard toRemove)
        {
            if (base.RemoveFromBoard(toRemove))
            {
                toRemove.ResetCard();
                return true;
            }
            return false;
        }
    }
}