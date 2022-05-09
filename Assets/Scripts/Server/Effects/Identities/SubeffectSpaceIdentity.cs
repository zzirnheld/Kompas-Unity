using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpaceIdentity : ContextInitializeableBase, IContextInitializeable
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
        /// <summary>
        /// Forwards on the logic to a GamestateSpaceIdentity
        /// </summary>
        public class FromGamestate : SubeffectSpaceIdentity
        {
            public GamestateSpaceIdentity spaceIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceIdentity.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => spaceIdentity.Space;
        }
    }
}