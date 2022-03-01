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

        private (IEnumerable<ActivationContext> moveContexts, IEnumerable<ActivationContext> leaveContexts)
            GetContextsForMove(GameCard card, Space from, Space to, Player player, IStackable stackSrc)
        {
            int distance = from.DistanceTo(to);

            var moveContexts = new List<ActivationContext>();
            var leaveContexts = new List<ActivationContext>();
            //Cards that from card is no longer in the AOE of
            var cardsMoverLeft = CardsAndAugsWhere(c => c != null && c.CardInAOE(card) && !c.SpaceInAOE(to));
            //Cards that from card no longer has in its aoe
            var cardsMoverLeftBehind = CardsAndAugsWhere(c => c != null && card.CardInAOE(c) && !card.CardInAOE(c, to));

            //Add contexts for 
            moveContexts.Add(new ActivationContext(mainCardBefore: card, stackable: stackSrc, space: to,
                player: player, x: distance));
            //Cards that from card is no longer in the AOE of
            leaveContexts.AddRange(cardsMoverLeft.Select(c =>
                new ActivationContext(mainCardBefore: card, secondaryCardBefore: c, stackable: stackSrc, player: player)));
            //Cards that from card no longer has in its aoe
            leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
                new ActivationContext(mainCardBefore: c, secondaryCardBefore: card, stackable: stackSrc, player: player)));
            //trigger for first card's augments
            foreach (var aug in card.Augments)
            {
                //Add contexts for 
                moveContexts.Add(new ActivationContext(mainCardBefore: aug, stackable: stackSrc, space: to,
                    player: player, x: distance));
                //Cards that from aug is no longer in the AOE of
                leaveContexts.AddRange(cardsMoverLeft.Select(c =>
                    new ActivationContext(mainCardBefore: aug, secondaryCardBefore: c, stackable: stackSrc, player: player)));
                //Cards that from aug no longer has in its aoe
                leaveContexts.AddRange(cardsMoverLeftBehind.Select(c =>
                    new ActivationContext(mainCardBefore: c, secondaryCardBefore: aug, stackable: stackSrc, player: player)));
            }
            return (moveContexts, leaveContexts);
        }

        protected override void Swap(GameCard card, Space to, bool playerInitiated, IStackable stackSrc = null)
        {
            //calculate distance before doing the swap
            var from = card.Position.Copy;
            var at = GetCardAt(to);
            var player = playerInitiated ? card.Controller : stackSrc?.Controller;

            //then trigger appropriate triggers. list of contexts:
            var moveContexts = new List<ActivationContext>();
            var leaveContexts = new List<ActivationContext>();

            var (fromCardMoveContexts, fromCardLeaveContexts) = GetContextsForMove(card, from, to, player, stackSrc);
            moveContexts.AddRange(fromCardMoveContexts);
            leaveContexts.AddRange(fromCardLeaveContexts);

            if (at != null)
            {
                var (atCardMoveContexts, atCardLeaveContexts) = GetContextsForMove(at, to, from, player, stackSrc);
                moveContexts.AddRange(atCardMoveContexts);
                leaveContexts.AddRange(atCardLeaveContexts);
            }

            //actually perform the swap
            base.Swap(card, to, playerInitiated);

            foreach (var ctxt in moveContexts)
            {
                ctxt.CacheCardInfoAfter();
            }

            EffectsController.TriggerForCondition(Trigger.Move, moveContexts.ToArray());
            EffectsController.TriggerForCondition(Trigger.Arrive, moveContexts.ToArray());
            EffectsController.TriggerForCondition(Trigger.LeaveAOE, leaveContexts.ToArray());

            //notify the players
            ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, to);
        }
    }
}