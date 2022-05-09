using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public interface ISubeffectManyNumbersIdentity : IContextInitializeable
    {
        public ICollection<int> Numbers { get; }
    }

    public abstract class SubeffectManyNumbersIdentityBase : ContextInitializeableBase,
        ISubeffectManyNumbersIdentity
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

    namespace SubeffectManyNumbersIdentities
    {
        public class Distances : SubeffectManyNumbersIdentityBase
        {
            public ISubeffectSpaceIdentity origin;
            public SubeffectManySpacesIdentity destinations;

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