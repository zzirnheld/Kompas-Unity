using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateNumbersIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract ICollection<int> NumbersLogic();

        public ICollection<int> Numbers() => initialized ? NumbersLogic()
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class DistancesNumbersIdentity : GamestateNumbersIdentity
    {
        public GamestateSpaceIdentity originIdentity;
        public GamestateSpacesIdentity destinationsIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            originIdentity.Initialize(restrictionContext);
            destinationsIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<int> NumbersLogic()
        {
            var origin = originIdentity.Space();
            var destinations = destinationsIdentity.Spaces();
            return destinations.Select(dest => origin.DistanceTo(dest)).ToArray();
        }
    }
}