using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
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

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            spaceRestriction.Initialize(restrictionContext.source, restrictionContext.source.Controller, restrictionContext.subeffect.Effect, restrictionContext.subeffect);
        }

        protected override ICollection<Space> GetSpacesLogic()
            => Space.Spaces.Where(s => spaceRestriction.IsValidSpace(s, default)).ToArray();
    }

    /// <summary>
    /// Spaces for whom the distance between them and a defined space, compared to the given number, is in some relationship.
    /// For example, spaces whose distance between them and this card's space (defined space), are greater than (relationship) X (the given number).
    /// <br/>
    /// NOTE: The comparison's LHS is the distance, and RHS is the number.
    /// </summary>
    public class DistanceNumberRelationshipSubeffectSpacesIdentity : SubeffectSpacesIdentity
    {
        public SubeffectSpaceIdentity originIdentity;
        public SubeffectNumberIdentity numberIdentity;
        public INumberRelationship numberRelationship;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            originIdentity.Initialize(restrictionContext);
            numberIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<Space> GetSpacesLogic()
        {
            var origin = originIdentity.Space;
            int number = numberIdentity.Number;

            return Space.Spaces.Where(s => numberRelationship.Compare(origin.DistanceTo(s), number)).ToArray();
        }
    }
}