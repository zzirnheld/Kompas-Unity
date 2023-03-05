using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class All : ContextlessLeafIdentityBase<IReadOnlyCollection<Space>>
        {
            protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.ToList();
        }

        public class FittingRestriction : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public SpaceRestriction restriction;
            public IIdentity<IReadOnlyCollection<Space>> spaces = new All();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => spaces.From(context, secondaryContext)
                    .Where(s => restriction.IsValidSpace(s, InitializationContext.effect?.CurrActivationContext)).ToList();
        }

        public class PositionsOfEach : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => cards.From(context, secondaryContext)
                        .Select(c => c.Position)
                        .Where(space => space != null)
                        .ToArray();
        }

        public class Corners : ContextlessLeafIdentityBase<IReadOnlyCollection<Space>>
        {
            protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.Where(s => s.IsCorner).ToArray();
        }

        public class Multiple : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<Space>[] spaces;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var i in spaces) i.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => spaces.Select(s => s.From(context, secondaryContext)).ToArray();
        }

        public class AdjacentSpaces : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<Space> adjacentTo;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                adjacentTo.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => adjacentTo.From(context, secondaryContext).AdjacentSpaces;
        }

        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<Space> firstSpace;
            public IIdentity<Space> secondSpace;

            public IThreeSpaceRelationship thirdSpaceRelationship;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstSpace.Initialize(initializationContext);
                secondSpace.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            {
                Space first = firstSpace.From(context, secondaryContext);
                Space second = secondSpace.From(context, secondaryContext);
                return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
            }
        }

        public class CompareDistance : ContextualIdentityBase<IReadOnlyCollection<Space>>
        {
            public IIdentity<IReadOnlyCollection<Space>> spaces;
            public IIdentity<Space> distanceTo;

            public bool closest = true;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
                distanceTo.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
            {
                var tuples = spaces.From(context, secondaryContext)
                    .Select(s => (s, s.DistanceTo(distanceTo.From(context, secondaryContext))))
                    .OrderBy(tuple => tuple.Item2);
                if (tuples.Count() == 0) return tuples.Select(tuple => tuple.s).ToList();
                
                int dist = tuples.First().Item2;
                return tuples.Where(tuple => tuple.Item2 == dist).Select(tuple => tuple.s).ToList();
            }
            
        }
    }
}