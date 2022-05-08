using System;
using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextCardIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract GameCardBase CardFromLogic(ActivationContext context);

        public GameCardBase CardFrom(ActivationContext context)
            => initialized ? CardFromLogic(context)
                : throw new NotImplementedException("You forgot to initialize an ActivationContextCardIdentity!");
    }

    public class ThisCardContextCardIdentity : ActivationContextCardIdentity
    {
        protected override GameCardBase CardFromLogic(ActivationContext context)
            => RestrictionContext.source;
    }

    public class MainCardBeforeCardIdentity : ActivationContextCardIdentity
    {
        protected override GameCardBase CardFromLogic(ActivationContext context)
            => context.mainCardInfoBefore;
    }

    public class GameContextCardIdentity : ActivationContextCardIdentity
    {
        public IGamestateCardIdentity gamestateCardIdentity;

        protected override GameCardBase CardFromLogic(ActivationContext context)
            => gamestateCardIdentity.CardFrom(context.game, context);
    }

    public class CardAtPositionContextCardIdentity : ActivationContextCardIdentity
    {
        public ActivationContextSpaceIdentity cardPositionIdentity;

        protected override GameCardBase CardFromLogic(ActivationContext context)
        {
            var finalSpace = cardPositionIdentity.SpaceFrom(context);
            return context.game.boardCtrl.GetCardAt(finalSpace);
        }
    }
}