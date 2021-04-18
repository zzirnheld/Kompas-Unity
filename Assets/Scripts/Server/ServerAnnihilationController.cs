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
            var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: stackSrc?.Controller);
            bool wasKnown = card.KnownToEnemy;
            if (base.Annihilate(card, stackSrc))
            {
                ServerGame.EffectsController.TriggerForCondition(Trigger.Annhilate, context);
                ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card, wasKnown);
                return true;
            }
            return false;
        }
    }
}