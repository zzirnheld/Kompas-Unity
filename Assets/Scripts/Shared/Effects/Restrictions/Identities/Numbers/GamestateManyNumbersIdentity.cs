using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateManyNumbersIdentity : ContextInitializeableBase, IContextInitializeable
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
            public GamestateSpaceIdentity originIdentity;
            public GamestateManySpacesIdentity destinationsIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                originIdentity.Initialize(restrictionContext);
                destinationsIdentity.Initialize(restrictionContext);
            }

            protected override ICollection<int> AbstractNumbers
            {
                get
                {
                    var origin = originIdentity.Space;
                    var destinations = destinationsIdentity.Spaces;
                    return destinations.Select(dest => origin.DistanceTo(dest)).ToArray();
                }
            }
        }
    }
}