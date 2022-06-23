using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateManyNumbersIdentities
    {
        public class Distances : NoActivationContextIdentityBase<ICollection<int>>
        {
            public INoActivationContextIdentity<Space> origin;
            public INoActivationContextIdentity<ICollection<Space>> destinations;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                origin.Initialize(initializationContext);
                destinations.Initialize(initializationContext);
            }

            protected override ICollection<int> AbstractItem
            {
                get
                {
                    var origin = this.origin.Item;
                    var destinations = this.destinations.Item;
                    return destinations.Select(dest => origin.DistanceTo(dest)).ToArray();
                }
            }
        }
    }
}