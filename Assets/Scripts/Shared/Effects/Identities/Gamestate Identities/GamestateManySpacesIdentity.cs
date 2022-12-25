using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class All : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.ToList();
        }

        public class FittingRestriction : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public SpaceRestriction restriction;
            public NoActivationContextIdentityBase<IReadOnlyCollection<Space>> spaces = new All();

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem
                => spaces.Item.Where(s => restriction.IsValidSpace(s, InitializationContext.effect?.CurrActivationContext)).ToList();
        }

        public class PositionsOfEach : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem => cards.Item
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }

        public class Corners : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            protected override IReadOnlyCollection<Space> AbstractItem => Space.Spaces.Where(s => s.IsCorner).ToArray();
        }

        public class Multiple : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public INoActivationContextIdentity<Space>[] spaces;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var i in spaces) i.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem => spaces.Select(s => s.Item).ToArray();
        }

        public class AdjacentSpaces : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public INoActivationContextIdentity<Space> adjacentTo;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                adjacentTo.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem => adjacentTo.Item.AdjacentSpaces;
        }

        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public INoActivationContextIdentity<Space> firstSpace;
            public INoActivationContextIdentity<Space> secondSpace;

            public IThreeSpaceRelationship thirdSpaceRelationship;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstSpace.Initialize(initializationContext);
                secondSpace.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem
            {
                get
                {
                    Space first = firstSpace.Item;
                    Space second = secondSpace.Item;
                    return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
                }
            }
        }

        public class CompareDistance : NoActivationContextIdentityBase<IReadOnlyCollection<Space>>
        {
            public INoActivationContextIdentity<IReadOnlyCollection<Space>> spaces;
            public INoActivationContextIdentity<Space> distanceTo;

            public bool closest = true;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaces.Initialize(initializationContext);
                distanceTo.Initialize(initializationContext);
            }

            protected override IReadOnlyCollection<Space> AbstractItem
            {
                get 
                {
                    var tuples = spaces.Item.Select(s => (s, s.DistanceTo(distanceTo.Item))).OrderBy(tuple => tuple.Item2);
                    if (tuples.Count() == 0) return tuples.Select(tuple => tuple.s).ToList();
                    
                    int dist = tuples.First().Item2;
                    return tuples.Where(tuple => tuple.Item2 == dist).Select(tuple => tuple.s).ToList();
                }
            }
            
        }
    }
}