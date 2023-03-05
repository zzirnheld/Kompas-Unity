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

        public class PositionsOfEach : SubeffectIdentityBase<ICollection<Space>>
        {
            public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

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

        public class Corners : SubeffectIdentityBase<ICollection<Space>>
        {
            protected override ICollection<Space> AbstractItem => new Space[] { (0, 0), (0, Space.MaxIndex), (Space.MaxIndex, 0), (Space.MaxIndex, Space.MaxIndex) };
        }
    }
}