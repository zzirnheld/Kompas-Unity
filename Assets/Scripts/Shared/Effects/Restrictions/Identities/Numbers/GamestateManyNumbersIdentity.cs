using KompasServer.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateManyNumbersIdentity : ContextInitializeableBase,
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

    namespace GamestateManyNumbersIdentities
    {
        public class Distances : GamestateManyNumbersIdentity
        {
            public GamestateSpaceIdentity origin;
            public GamestateManySpacesIdentity destinations;

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