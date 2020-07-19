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
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: stackSrc?.Controller);
                ServerGame.EffectsController.TriggerForCondition(Trigger.Annhilate, context);
                ServerGame.ServerPlayers[card.ControllerIndex].ServerNotifier.NotifyAnnhilate(card);
                return base.Annihilate(card, stackSrc);
            }
            return false;
        }
    }
}