using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpaceIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }
        protected abstract Space AbstractSpace { get; }

        public Space Space => initialized ? AbstractSpace
            : throw new NotImplementedException("You forgot to initialize a SubeffectCardIdentity!");

    }

    /// <summary>
    /// Forwards on the logic to a GamestateSpaceIdentity
    /// </summary>
    public class GamestateSubeffectSpaceIdentity : SubeffectSpaceIdentity
    {
        public GamestateSpaceIdentity spaceIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            spaceIdentity.Initialize(restrictionContext);
        }

        protected override Space AbstractSpace => spaceIdentity.Space();
    }
}