using KompasCore.Effects;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public interface ISubeffectManySpacesIdentity : IContextInitializeable
    {
        public ICollection<Space> Spaces { get; }
    }

    public abstract class SubeffectManySpacesIdentity : ContextInitializeableBase, ISubeffectManySpacesIdentity
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
                spaceRestriction.Initialize(restrictionContext.source, restrictionContext.source.Controller, restrictionContext.subeffect.Effect, restrictionContext.subeffect);
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
            public ISubeffectSpaceIdentity firstSpace;
            public ISubeffectSpaceIdentity secondSpace;

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
            public GamestateCardsIdentity cards;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cards.Initialize(restrictionContext);
            }

            protected override ICollection<Space> AbstractSpaces
                => cards.CardsFrom(RestrictionContext.game).Select(c => c.Position).ToArray();
        }
    }
}