using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpaceIdentityBase : SubeffectInitializeableBase,
        INoActivationContextSpaceIdentity
    {
        protected abstract Space AbstractSpace { get; }

        public Space Space
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpace;
            }
        }
    }

    namespace SubeffectSpaceIdentities
    {
        public class PositionOf : SubeffectSpaceIdentityBase
        {
            public INoActivationContextCardIdentity whosePosition;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                whosePosition.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => whosePosition.Card.Position;
        }
    }
}