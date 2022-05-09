using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public interface ISubeffectSpaceIdentity : IContextInitializeable
    {
        public Space Space { get; }
    }

    public abstract class SubeffectSpaceIdentityBase : ContextInitializeableBase, ISubeffectSpaceIdentity
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
        public class FromGamestate : SubeffectSpaceIdentityBase
        {
            public GamestateSpaceIdentity spaceFromGamestate;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceFromGamestate.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => spaceFromGamestate.Space;
        }

        public class PositionOf : SubeffectSpaceIdentityBase
        {
            public ISubeffectCardIdentity whosePosition;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                whosePosition.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => whosePosition.Card.Position;
        }
    }
}