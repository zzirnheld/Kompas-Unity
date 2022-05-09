using KompasCore.Cards;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextCardIdentity : ContextInitializeableBase, IContextInitializeable
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
        public class ThisCard : ActivationContextCardIdentity
        {
            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => RestrictionContext.source;
        }

        public class MainCardBefore : ActivationContextCardIdentity
        {
            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => context.mainCardInfoBefore;
        }

        public class FromGamestate : ActivationContextCardIdentity
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

        public class CardAtPosition : ActivationContextCardIdentity
        {
            public ActivationContextSpaceIdentity position;

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