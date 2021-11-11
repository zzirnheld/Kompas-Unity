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

        protected override void AddCard(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(beforeCard: card, stackable: stackSrc, triggerer: Owner);
            base.AddCard(card);
            context.SetAfterCardInfo(card);
            EffectsController.TriggerForCondition(Trigger.ToDeck, context);
            owner.ServerNotifier.NotifyDeckCount(Deck.Count);
        }

        public override void PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(beforeCard: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            base.PushBottomdeck(card, stackSrc);
            context.SetAfterCardInfo(card);
            EffectsController.TriggerForCondition(Trigger.Bottomdeck, context);
            ServerNotifier.NotifyBottomdeck(card, wasKnown);
        }

        public override void PushTopdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(beforeCard: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            base.PushTopdeck(card, stackSrc);
            context.SetAfterCardInfo(card);
            EffectsController.TriggerForCondition(Trigger.Topdeck, context);
            ServerNotifier.NotifyTopdeck(card, wasKnown);
        }

        public override void ShuffleIn(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(beforeCard: card, stackable: stackSrc, triggerer: Owner);
            bool wasKnown = card.KnownToEnemy;
            base.ShuffleIn(card, stackSrc);
            context.SetAfterCardInfo(card);
            EffectsController.TriggerForCondition(Trigger.Reshuffle, context);
            ServerNotifier.NotifyReshuffle(card, wasKnown);
        }

        public override void Remove(GameCard card)
        {
            base.Remove(card);
            owner.ServerNotifier.NotifyDeckCount(Deck.Count);
        }
    }
}