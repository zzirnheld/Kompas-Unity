using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerDiscardController : DiscardController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        public override bool AddToDiscard(GameCard card, IStackable stackSrc = null)
        {
            if (!card.CanRemove) return false;
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            EffectsController.TriggerForCondition(Trigger.Discard, context);
            ServerNotifier.NotifyDiscard(card);
            return base.AddToDiscard(card);
        }

        public override bool RemoveFromDiscard(GameCard card)
        {
            if(base.RemoveFromDiscard(card))
            {
                card.ResetCard();
                return true;
            }
            return false;
        }
    }
}