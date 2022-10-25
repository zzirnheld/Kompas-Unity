using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectManySpacesIdentities
    {
        public class ByRestriction : SubeffectIdentityBase<ICollection<Space>>
        {
            public SpaceRestriction spaceRestriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                spaceRestriction.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractItem
                => Space.Spaces.Where(s => spaceRestriction.IsValidSpace(s, default)).ToArray();
        }

        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : SubeffectIdentityBase<ICollection<Space>>
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

            protected override ICollection<Space> AbstractItem
            {
                get
                {
                    Space first = firstSpace.Item;
                    Space second = secondSpace.Item;
                    return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
                }
            }
        }

        public class PositionsOfEach : SubeffectIdentityBase<ICollection<Space>>
        {
            public INoActivationContextIdentity<ICollection<GameCardBase>> cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractItem => cards.Item
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }
    }
}