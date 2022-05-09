using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextCardIdentity : IContextInitializeable
    {
        public GameCardBase CardFrom(ActivationContext context);
    }

    public abstract class ActivationContextCardIdentityBase : ContextInitializeableBase, IActivationContextCardIdentity
    {
        public GameCardBase CardFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return CardFromAbstract(context);
        }

        protected abstract GameCardBase CardFromAbstract(ActivationContext context);
    }

    namespace ActivationContextCardIdentities
    {
        public class ThisCard : ActivationContextCardIdentityBase
        {
            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => RestrictionContext.source;
        }

        public class MainCardBefore : ActivationContextCardIdentityBase
        {
            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => context.mainCardInfoBefore;
        }

        public class FromGamestate : ActivationContextCardIdentityBase
        {
            public GamestateCardIdentity cardFromGamestate;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardFromGamestate.Initialize(restrictionContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => cardFromGamestate.CardFrom(context.game, context);
        }

        public class CardAtPosition : ActivationContextCardIdentityBase
        {
            public IActivationContextSpaceIdentity position;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                position.Initialize(restrictionContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
            {
                var finalSpace = position.SpaceFrom(context);
                return context.game.boardCtrl.GetCardAt(finalSpace);
            }
        }
    }
}