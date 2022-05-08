using KompasCore.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectSpacesIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        public ICollection<Space> GetSpaces() => initialized ? GetSpacesLogic()
            : throw new NotImplementedException("You forgot to initialize a SubeffectCardIdentity!");


        protected abstract ICollection<Space> GetSpacesLogic();
    }

    public class RestrictionSubeffectSpacesIdentity : SubeffectSpacesIdentity
    {
        public SpaceRestriction spaceRestriction;

        protected override ICollection<Space> GetSpacesLogic()
            => Space.Spaces.Where(s => spaceRestriction.IsValidSpace(s, default)).ToArray();
    }
}