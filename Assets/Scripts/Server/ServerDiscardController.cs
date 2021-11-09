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

        public override void Add(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            base.Add(card, stackSrc);
            EffectsController.TriggerForCondition(Trigger.Discard, context);
            ServerNotifier.NotifyDiscard(card, wasKnown);
        }
    }
}