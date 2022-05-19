using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectManyNumbersIdentityBase : SubeffectInitializeableBase,
        INoActivationContextIdentity<ICollection<int>>
    {
        protected abstract ICollection<int> AbstractNumbers { get; }

        public ICollection<int> Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractNumbers;
            }
        }
    }

    namespace SubeffectManyNumbersIdentities
    {
        public class Distances : SubeffectManyNumbersIdentityBase
        {
            public INoActivationContextIdentity<Space> origin;
            public INoActivationContextIdentity<ICollection<Space>> destinations;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                origin.Initialize(initializationContext);
                destinations.Initialize(initializationContext);
            }

            protected override ICollection<int> AbstractNumbers
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