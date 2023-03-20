using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Networking;

namespace KompasServer.GameCore
{
    public class ServerDeckController : DeckController
    {
        public ServerPlayer owner;

        public override Player Owner => Owner;
        
        public ServerGame ServerGame => owner.game;
        public ServerNotifier ServerNotifier => ServerGame.serverPlayers[Owner.index].notifier;
        public ServerEffectsController EffectsController => ServerGame.effectsController;

        protected override bool AddToDeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: Owner);
            bool successfulAdd = base.AddToDeck(card);
            if (successfulAdd)
            {
                context.CacheCardInfoAfter();
                EffectsController.TriggerForCondition(Trigger.ToDeck, context);
                owner.notifier.NotifyDeckCount(Cards.Count());
            }
            return successfulAdd;
        }

        public override bool PushBottomdeck(GameCard card, IStackable stackSrc = null)
        {
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: Owner);
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
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: Owner);
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
            var context = new ActivationContext(game: ServerGame, mainCardBefore: card, stackableCause: stackSrc, player: Owner);
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
            owner.notifier.NotifyDeckCount(Cards.Count());
        }
    }
}