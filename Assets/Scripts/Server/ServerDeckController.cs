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

        protected override bool AddCard(GameCard card, IStackable stackSrc = null)
        {
            if (card.CanRemove)
            {
                var context = new ActivationContext(card: card, stackable: stackSrc, triggerer: Owner);
                EffectsController.TriggerForCondition(Trigger.ToDeck, context);
                var success = base.AddCard(card);
                owner.ServerNotifier.NotifyDeckCount(Deck.Count);
                return success;
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

        public override bool RemoveFromDeck(GameCard card)
        {
            if(base.RemoveFromDeck(card))
            {
                card.ResetCard();
                owner.ServerNotifier.NotifyDeckCount(Deck.Count);
                return true;
            }
            return false;
        }
    }
}