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
            public GamestateSpaceIdentity spaceFromGamestate;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceFromGamestate.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => spaceFromGamestate.Space;
        }

        public class PositionOf : SubeffectSpaceIdentity
        {
            public SubeffectCardIdentity whosePosition;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                whosePosition.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => whosePosition.Card.Position;
        }
    }
}