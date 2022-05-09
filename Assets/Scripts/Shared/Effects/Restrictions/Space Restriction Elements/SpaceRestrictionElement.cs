using KompasCore.Effects.Relationships;
using KompasServer.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{
    public abstract class SpaceRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool IsValidSpace(Space space, ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidSpace(space, context);
        }

        protected abstract bool AbstractIsValidSpace(Space space, ActivationContext context);
    }

    namespace SpaceRestrictionElements
    {
        /// <summary>
        /// Gets the distance between the described origin point and the space to be tested,
        /// gets the described number,
        /// and compares the distance to the number with the given comparison.
        /// </summary>
        public class CompareDistance : SpaceRestrictionElement
        {
            public SubeffectSpaceIdentity distanceTo;
            public SubeffectNumberIdentity number;
            public INumberRelationship comparison;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                distanceTo.Initialize(restrictionContext);
                number.Initialize(restrictionContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var origin = this.distanceTo.Space;
                int distance = origin.DistanceTo(space);

                int number = this.number.Number;

                return comparison.Compare(distance, number);
            }
        }
    }
}