using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectManySpacesIdentity : SubeffectInitializeableBase,
        INoActivationContextManySpacesIdentity
    {
        public ICollection<Space> Spaces
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpaces;
            }
        }

        protected abstract ICollection<Space> AbstractSpaces { get; }
    }

    namespace SubeffectManySpacesIdentities
    {
        public class ByRestriction : SubeffectManySpacesIdentity
        {
            public SpaceRestriction spaceRestriction;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                spaceRestriction.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpaces
                => Space.Spaces.Where(s => spaceRestriction.IsValidSpace(s, default)).ToArray();
        }

        /// <summary>
        /// Spaces where they are in some defined relationship with respect to the other two defined spaces.
        /// For example, spaces that are between (relationship) the source card's space and the target space (two defined spaces).
        /// </summary>
        public class ThreeSpaceRelationship : SubeffectManySpacesIdentity
        {
            public INoActivationContextSpaceIdentity firstSpace;
            public INoActivationContextSpaceIdentity secondSpace;

            public IThreeSpaceRelationship thirdSpaceRelationship;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                firstSpace.Initialize(restrictionContext);
                secondSpace.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpaces
            {
                get
                {
                    Space first = firstSpace.Space;
                    Space second = secondSpace.Space;
                    return Space.Spaces.Where(space => thirdSpaceRelationship.Evaluate(first, second, space)).ToArray();
                }
            }
        }

        public class PositionsOfEach : SubeffectManySpacesIdentity
        {
            public INoActivationContextManyCardsIdentity cards;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cards.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpaces => cards.Cards
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }
    }
}