using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManySpacesIdentities
    {
        public class PositionsOfEach : NoActivationContextIdentityBase<ICollection<Space>>
        {
            public INoActivationContextManyCardsIdentity cards;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                cards.Initialize(initializationContext);
            }

            protected override ICollection<Space> AbstractItem => cards.Cards
                .Select(c => c.Position)
                .Where(space => space != null)
                .ToArray();
        }

        public class Corners : NoActivationContextIdentityBase<ICollection<Space>>
        {
            protected override ICollection<Space> AbstractItem => Space.Spaces.Where(s => s.IsCorner).ToArray();
        }
    }
}