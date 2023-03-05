using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectCardIdentities
    {
        public class FromActivationContext : SubeffectIdentityBase<GameCardBase>
        {
            public IIdentity<GameCardBase> cardFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cardFromContext.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItem =>
                cardFromContext.From(InitializationContext.subeffect.CurrentContext, default);
        }

        /// <summary>
        /// Gets a card according to a target index.
        /// Negative indices index from the end of the targets array,
        /// so target -1 is the last target chosen.
        /// </summary>
        public class Target : SubeffectIdentityBase<GameCardBase>
        {
            public int index = -1;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                if (index == -1) index = initializationContext.subeffect.targetIndex;
            }

            protected override GameCardBase AbstractItem => InitializationContext.subeffect.Effect.GetTarget(index);
        }
    }
}