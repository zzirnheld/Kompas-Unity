using KompasServer.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextManyNumbersIdentity : IContextInitializeable
    {
        public ICollection<int> Numbers { get; }
    }

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