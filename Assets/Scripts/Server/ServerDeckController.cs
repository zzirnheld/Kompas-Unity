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

        public override Player Owner => owner;
        public ServerPlayer owner;

        protected override bool AddToDeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool successfulAdd = base.AddToDeck(card);
            if (successfulAdd)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.ToDeck, context);
                owner.ServerNotifier.NotifyDeckCount(Deck.Count);
            }
            return successfulAdd;
        }

        public override bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool wasKnown = card.KnownToEnemy;
            bool successful = base.PushBottomdeck(card, stackSrc);
            if (successful)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.Bottomdeck, context);
                ServerNotifier.NotifyBottomdeck(card, wasKnown);
            }
            return successful;
        }

        public override bool PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool wasKnown = card.KnownToEnemy;
            bool successful = base.PushTopdeck(card, stackSrc);
            if (successful)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.Topdeck, context);
                ServerNotifier.NotifyTopdeck(card, wasKnown);
            }
            return successful;
        }

        public override bool ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool wasKnown = card.KnownToEnemy;
            bool successful = base.ShuffleIn(card, stackSrc);
            if (successful)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.Reshuffle, context);
                ServerNotifier.NotifyReshuffle(card, wasKnown);
            }
            return successful;
        }

        public override void Remove(GameCard card)
        {
            base.Remove(card);
            owner.ServerNotifier.NotifyDeckCount(Deck.Count);
        }
    }
}