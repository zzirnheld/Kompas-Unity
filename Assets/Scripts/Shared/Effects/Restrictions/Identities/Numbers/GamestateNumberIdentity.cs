namespace KompasCore.Effects.Identities
{
    public abstract class GamestateNumberIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract int AbstractNumber { get; }

        public int Number
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractNumber;
            }
        }
    }

    namespace GamestateNumberIdentities
    {
        public class Selector : GamestateNumberIdentity
        {
            public INumberSelector selector;
            public GamestateManyNumbersIdentity numbers;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                numbers.Initialize(restrictionContext);
            }

            protected override int AbstractNumber
                => selector.Apply(numbers.Numbers);
        }
    }
}