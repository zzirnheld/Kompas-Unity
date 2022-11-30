using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class PositionsOfEach : NoActivationContextIdentityBase<ICollection<Space>>
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

        public class Corners : NoActivationContextIdentityBase<ICollection<Space>>
        {
            protected override ICollection<Space> AbstractItem => Space.Spaces.Where(s => s.IsCorner).ToArray();
        }

        public class Multiple : NoActivationContextIdentityBase<ICollection<Space>>
        {
            public INoActivationContextIdentity<Space>[] spaces;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var i in spaces) i.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractItem => spaces.Select(s => s.Item).ToArray();
        }
    }
}