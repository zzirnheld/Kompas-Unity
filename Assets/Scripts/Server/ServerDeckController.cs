using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerDeckController : DeckController
    {
        public ServerGame ServerGame;

        public ServerNotifier ServerNotifier => ServerGame.ServerPlayers[Owner.index].ServerNotifier;
        public ServerEffectsController EffectsController => ServerGame.EffectsController;

        protected override bool AddCard(GameCard card, IStackable stackSrc = null)
        {
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
                EffectsController.TriggerForCondition(Trigger.ToDeck, context);
                return base.AddCard(card);
            }
            return false;
        }

        public override bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
                EffectsController.TriggerForCondition(Trigger.Bottomdeck, context);
                ServerNotifier.NotifyBottomdeck(card);
                return base.PushBottomdeck(card, stackSrc);
            }
            return false;
        }

        public override bool PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
                EffectsController.TriggerForCondition(Trigger.Topdeck, context);
                ServerNotifier.NotifyTopdeck(card);
                return base.PushTopdeck(card, stackSrc);
            }
            return false;
        }

        public override bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
                EffectsController.TriggerForCondition(Trigger.Reshuffle, context);
                ServerNotifier.NotifyReshuffle(card);
                return base.ShuffleIn(card, stackSrc);
            }
            return false;
        }
    }
}