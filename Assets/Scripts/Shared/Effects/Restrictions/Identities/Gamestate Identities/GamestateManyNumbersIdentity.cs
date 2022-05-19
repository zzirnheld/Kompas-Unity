using KompasServer.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Identifies a collection of numbers, like distances or stats of a whole bunch of cards.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateManyNumbersIdentityBase : ContextInitializeableBase,
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

    namespace GamestateManyNumbersIdentities
    {
        public class Distances : GamestateManyNumbersIdentityBase
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