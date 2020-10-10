using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerBoardController : BoardController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifierByIndex(int index) => ServerGame.ServerPlayers[index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        public override bool Play(GameCard toPlay, int toX, int toY, Player controller, IStackable stackSrc = null)
        {
            if (toPlay.CanRemove)
            {
                var context = new ActivationContext(card: toPlay, stackable: stackSrc, triggerer: controller, space: (toX, toY));
                EffectsController.TriggerForCondition(Trigger.Play, context);
                EffectsController.TriggerForCondition(Trigger.Arrive, context);
                if (!toPlay.IsAvatar) ServerNotifierByIndex(toPlay.ControllerIndex).NotifyPlay(toPlay, toX, toY);
                return base.Play(toPlay, toX, toY, controller);
            }
            return false;
        }

        public override bool Swap(GameCard card, int toX, int toY, bool playerInitiated, IStackable stackSrc = null)
        {
            if (card.IsAvatar && !card.Summoned) return false;
            if (card.Location != CardLocation.Field) return false;
            var contextA = new ActivationContext(card: card, stackable: stackSrc, space: (toX, toY),
                triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: card.DistanceTo(toX, toY));
            EffectsController.TriggerForCondition(Trigger.Move, contextA);
            EffectsController.TriggerForCondition(Trigger.Arrive, contextA);
            ServerNotifierByIndex(card.ControllerIndex).NotifyMove(card, toX, toY);
            var at = GetCardAt(toX, toY);
            if (at != null)
            {
                var contextB = new ActivationContext(card: at, stackable: stackSrc, space: (toX, toY),
                    triggerer: playerInitiated ? card.Controller : stackSrc?.Controller, x: at.DistanceTo(card.BoardX, card.BoardY));
                EffectsController.TriggerForCondition(Trigger.Move, contextB);
                EffectsController.TriggerForCondition(Trigger.Arrive, contextB);
            }
            return base.Swap(card, toX, toY, playerInitiated);
        }
    }
}