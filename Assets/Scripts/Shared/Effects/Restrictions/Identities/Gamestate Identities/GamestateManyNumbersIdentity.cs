using KompasServer.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextManyNumbersIdentity : IContextInitializeable
    {
        public ICollection<int> Numbers { get; }
    }

    /// <summary>
    /// Identifies a collection of numbers, like distances or stats of a whole bunch of cards.
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// </summary>
    public abstract class GamestateManyNumbersIdentityBase : ContextInitializeableBase,
        INoActivationContextManyNumbersIdentity
    {
        protected abstract ICollection<int> AbstractNumbers { get; }

        public ICollection<int> Numbers
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
            public INoActivationContextSpaceIdentity origin;
            public INoActivationContextManySpacesIdentity destinations;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                origin.Initialize(restrictionContext);
                destinations.Initialize(restrictionContext);
            }

            protected override ICollection<int> AbstractNumbers
            {
                get
                {
                    var origin = this.origin.Space;
                    var destinations = this.destinations.Spaces;
                    return destinations.Select(dest => origin.DistanceTo(dest)).ToArray();
                }
            }
        }
    }
}