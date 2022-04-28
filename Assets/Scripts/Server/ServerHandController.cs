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

        public override bool Hand(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackable: stackSrc, player: Owner);
            bool wasKnown = card.KnownToEnemy;
            bool successful = base.Hand(card, stackSrc);
            if (successful)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.Rehand, context);
                ServerNotifier.NotifyRehand(card, wasKnown);
            }
            return successful;
        }
    }
}