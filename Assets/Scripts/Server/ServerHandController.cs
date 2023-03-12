using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerHandController : HandController
    {
        public ServerPlayer owner;
        public override Player Owner => owner;

        public ServerGame ServerGame => owner.serverGame;

        public override bool Hand(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool wasKnown = card.KnownToEnemy;
            bool successful = base.Hand(card, stackSrc);
            if (successful)
            {
                context.CacheCardInfoAfter();
                ServerGame.effectsController.TriggerForCondition(Trigger.Rehand, context);
                owner.ServerNotifier.NotifyRehand(card, wasKnown);
            }
            return successful;
        }
    }
}