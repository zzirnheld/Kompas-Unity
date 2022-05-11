using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectCardIdentityBase : SubeffectInitializeableBase,
        INoActivationContextCardIdentity
    {
        public GameCardBase Card
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractCard;
            }
        }

        protected abstract GameCardBase AbstractCard { get; }
    }

    namespace SubeffectCardIdentities
    {
        public class FromActivationContext : SubeffectCardIdentityBase
        {
            public IActivationContextCardIdentity cardFromContext;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardFromContext.Initialize(restrictionContext);
            }

            protected override GameCardBase AbstractCard
                => cardFromContext.CardFrom(RestrictionContext.subeffect.CurrentContext);
        }

        /// <summary>
        /// Gets a card according to a target index.
        /// Negative indices index from the end of the targets array,
        /// so target -1 is the last target chosen.
        /// </summary>
        public class Target : SubeffectCardIdentityBase
        {
            public int index = -1;

            protected override GameCardBase AbstractCard
                => RestrictionContext.subeffect.Effect.GetTarget(index);
        }
    }
}