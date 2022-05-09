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
            public SubeffectSpaceIdentity originIdentity;
            public SubeffectNumberIdentity numberIdentity;
            public INumberRelationship numberRelationship;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                originIdentity.Initialize(restrictionContext);
                numberIdentity.Initialize(restrictionContext);
            }

            protected override bool AbstractIsValidSpace(Space space, ActivationContext context)
            {
                var origin = originIdentity.Space;
                int distance = origin.DistanceTo(space);

                int number = numberIdentity.Number;

                return numberRelationship.Compare(distance, number);
            }
        }
    }
}