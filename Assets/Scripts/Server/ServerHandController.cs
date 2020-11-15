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
            if (!card.CanRemove) return false;
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.TriggerForCondition(Trigger.Rehand, context);
            ServerNotifier.NotifyRehand(card);
            bool success = base.AddToHand(card);
            if(success) card.ResetCard();
            return success;
        }
    }
}