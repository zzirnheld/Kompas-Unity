using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    /// <summary>
    /// Gets the distance between the described origin point and the space to be tested,
    /// gets the described number,
    /// and compares the distance to the number with the given comparison.
    /// </summary>
    public class CompareDistance : SpaceRestrictionElement
    {
        public INoActivationContextIdentity<Space> distanceTo;
        public INoActivationContextIdentity<int> number;
        public INumberRelationship comparison;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            distanceTo.Initialize(initializationContext);
            number.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
        {
            var origin = this.distanceTo.Item;
            int distance = origin.DistanceTo(space);

            int number = this.number.Item;

            return comparison.Compare(distance, number);
        }
    }
}