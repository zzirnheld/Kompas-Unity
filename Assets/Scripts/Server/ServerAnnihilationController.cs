using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;

namespace KompasServer.GameCore
{
    public class ServerAnnihilationController : AnnihilationController
    {
        public ServerGame ServerGame;

        public override bool Annihilate(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackableCause: stackSrc, player: stackSrc?.Controller);
            bool wasKnown = card.KnownToEnemy;
            bool actuallyAnnihilated = base.Annihilate(card, stackSrc);
            if (actuallyAnnihilated)
            {
                ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card, wasKnown);
                context.CacheCardInfoAfter();
                ServerGame.EffectsController.TriggerForCondition(Trigger.Annhilate, context);
            }
            return actuallyAnnihilated;
        }
    }
}