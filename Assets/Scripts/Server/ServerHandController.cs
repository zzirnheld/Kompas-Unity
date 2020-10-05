using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerHandController : HandController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        public override bool AddToHand(GameCard card, IStackable stackSrc = null)
        {
            if (Hand.Count >= MaxHandSize || !card.CanRemove) return false;
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.TriggerForCondition(Trigger.Rehand, context);
            ServerNotifier.NotifyRehand(card);
            return base.AddToHand(card);
        }
    }
}