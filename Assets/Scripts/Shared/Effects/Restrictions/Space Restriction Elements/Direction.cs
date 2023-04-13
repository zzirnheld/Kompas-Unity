using System;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    /// <summary>
    /// Gets the distance between the described origin point and the space to be tested,
    /// gets the described number,
    /// and compares the distance to the number with the given comparison.
    /// </summary>
    public class Direction : SpaceRestrictionElement
    {
        //One of these should be non-null. The other one will be replaced by the space to be tested
        public IIdentity<Space> from;
        public IIdentity<Space> to;

        public IIdentity<Space> directionToMatch = new Identities.Spaces.TargetIndex();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from?.Initialize(initializationContext);
            to?.Initialize(initializationContext);

            if (from != null && to != null) throw new ArgumentException("Specified both 'from' and 'to' spaces in direction restriction - what direction are we testing?");
            if (from == null && to == null) throw new ArgumentException("Specified neither 'from' nor 'to' spaces in direction restriction - what direction are we testing?");
        }

        protected override bool IsValidLogic(Space space, IResolutionContext context)
        {
            var origin = from?.From(context, default) ?? space;
            var destination = to?.From(context, default) ?? space;

            return origin.DirectionFromThisTo(destination) == directionToMatch.From(context, default);
        }
    }
}