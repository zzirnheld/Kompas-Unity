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

        public class FromGame : ActivationContextCardIdentity
        {
            public GamestateCardIdentity gamestateCardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                gamestateCardIdentity.Initialize(restrictionContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
                => gamestateCardIdentity.CardFrom(context.game, context);
        }

        public class CardAtPosition : ActivationContextCardIdentity
        {
            public ActivationContextSpaceIdentity cardPositionIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardPositionIdentity.Initialize(restrictionContext);
            }

            protected override GameCardBase CardFromAbstract(ActivationContext context)
            {
                var finalSpace = cardPositionIdentity.SpaceFrom(context);
                return context.game.boardCtrl.GetCardAt(finalSpace);
            }
        }
    }
}