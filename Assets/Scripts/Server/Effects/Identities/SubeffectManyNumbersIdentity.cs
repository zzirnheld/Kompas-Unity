using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    namespace SubeffectManyNumbersIdentities
    {
        public class Distances : SubeffectIdentityBase<ICollection<int>>
        {
            public IIdentity<Space> origin;
            public IIdentity<IReadOnlyCollection<Space>> destinations;

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