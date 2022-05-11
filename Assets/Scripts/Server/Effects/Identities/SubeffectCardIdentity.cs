using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectCardIdentityBase : SubeffectInitializeableBase,
        INoActivationContextIdentity<GameCardBase>
    {
        public GameCardBase Item
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
            public IActivationContextIdentity<GameCardBase> cardFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardFromContext.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractCard
                => cardFromContext.From(InitializationContext.subeffect.CurrentContext);
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
                => InitializationContext.subeffect.Effect.GetTarget(index);
        }
    }
}