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
            var context = new ActivationContext(mainCardBefore: toPlay, stackable: stackSrc, player: controller, space: to);
            bool wasKnown = toPlay.KnownToEnemy;
            base.Play(toPlay, to, controller);
            context.CacheCardInfoAfter();
            EffectsController.TriggerForCondition(Trigger.Play, context);
            EffectsController.TriggerForCondition(Trigger.Arrive, context);
            if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, to, wasKnown);
        }

        public override void Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            //calculate distance before doing the swap
            int distance = card.DistanceTo(to);
            var at = GetCardAt(to);

            //then trigger appropriate triggers. list of contexts:
            List<ActivationContext> ctxts = new List<ActivationContext>();
            //trigger for first card
            ctxts.Add(new ActivationContext(mainCardBefore: card, stackable: stackSrc, space: to,
                player: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));
            //trigger for first card's augments
            foreach (var aug in card.Augments)
            {
                ctxts.Add(new ActivationContext(mainCardBefore: aug, stackable: null, space: to,
                    player: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
            }

            if (at != null)
            {
                //then trigger this card's triggers
                ctxts.Add(new ActivationContext(mainCardBefore: at, stackable: stackSrc, space: to,
                    player: playerInitiated ? card.Controller : stackSrc?.Controller, x: distance));

                //trigger for first card's augments
                foreach (var aug in at.Augments)
                {
                    ctxts.Add(new ActivationContext(mainCardBefore: aug, stackable: null, space: to,
                        player: playerInitiated ? aug.Controller : stackSrc?.Controller, x: distance));
                }
            }

            //actually perform the swap
            base.Swap(card, to, playerInitiated);

            foreach(var ctxt in ctxts)
            {
                ctxt.CacheCardInfoAfter();
            }

            EffectsController.TriggerForCondition(Trigger.Move, ctxts.ToArray());
            EffectsController.TriggerForCondition(Trigger.Arrive, ctxts.ToArray());

            //notify the players
            ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, to);
        }
    }
}