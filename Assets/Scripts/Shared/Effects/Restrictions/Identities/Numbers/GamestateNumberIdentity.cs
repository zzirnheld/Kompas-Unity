namespace KompasCore.Effects.Identities
{
    public abstract class GamestateNumberIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract int NumberLogic { get; }

        public int Number => initialized ? NumberLogic
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextSpaceIdentity!");
    }

    public class SelectorNumberIdentity : GamestateNumberIdentity
    {
        public INumberSelector numberSelector;
        public GamestateNumbersIdentity numbersIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            numbersIdentity.Initialize(restrictionContext);
        }

        protected override int NumberLogic
            => numberSelector.Apply(numbersIdentity.Numbers());
    }
}