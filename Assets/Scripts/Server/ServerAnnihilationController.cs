using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;

namespace KompasServer.GameCore
{
    public class ServerAnnihilationController : AnnihilationController
    {
        public ServerPlayer owner;

        public override Player Owner => owner;

        public ServerGame ServerGame => owner.game;

        public override bool Annihilate(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: stackSrc?.Controller);
            bool wasKnown = card.KnownToEnemy;
            bool actuallyAnnihilated = base.Annihilate(card, stackSrc);
            if (actuallyAnnihilated)
            {
                ServerGame.serverPlayers[card.ControllerIndex].notifier.NotifyAnnhilate(card, wasKnown);
                context.CacheCardInfoAfter();
                ServerGame.effectsController.TriggerForCondition(Trigger.Annhilate, context);
            }
            return actuallyAnnihilated;
        }
    }
}