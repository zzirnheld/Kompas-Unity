using KompasCore.Effects.Relationships;
using KompasServer.Effects.Identities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
    public abstract class SpaceRestrictionElement
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        public bool FitsRestriction(Space space, ActivationContext context) => initialized ? FitsRestrictionLogic(space, context)
            : throw new System.NotImplementedException("You failed to initialize a Card Restriction Element");

        protected abstract bool FitsRestrictionLogic(Space space, ActivationContext context);
    }

    /// <summary>
    /// Gets the distance between the described origin point and the space to be tested,
    /// gets the described number,
    /// and compares the distance to the number with the given comparison.
    /// </summary>
    public class DistanceToCardSpaceRestrictionElement : SpaceRestrictionElement
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

        protected override bool FitsRestrictionLogic(Space space, ActivationContext context)
        {
            var origin = originIdentity.Space;
            int distance = origin.DistanceTo(space);

            int number = numberIdentity.Number;

            return numberRelationship.Compare(distance, number);
        }
    }
}