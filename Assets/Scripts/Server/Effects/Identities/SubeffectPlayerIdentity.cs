using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectPlayerIdentities
    {
        public class FromActivationContext : SubeffectIdentityBase<Player>
        {
            public IIdentity<Player> playerFromContext;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                playerFromContext.Initialize(initializationContext);
            }

            protected override Player AbstractItem =>
                playerFromContext.From(InitializationContext.subeffect.CurrentContext, default);
        }

        /// <summary>
        /// Gets a card according to a target index.
        /// Negative indices index from the end of the targets array,
        /// so target -1 is the last target chosen.
        /// </summary>
        public class Target : SubeffectIdentityBase<Player>
        {
            public int index = -1;

            protected override Player AbstractItem => InitializationContext.subeffect.Effect.GetPlayer(index);
        }
    }
}