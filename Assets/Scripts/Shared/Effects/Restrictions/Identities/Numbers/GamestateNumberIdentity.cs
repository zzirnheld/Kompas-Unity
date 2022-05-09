namespace KompasCore.Effects.Identities
{
    public abstract class GamestateNumberIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract int NumberLogic { get; }

        public int Number
        {
            get
            {
                ComplainIfNotInitialized();
                return NumberLogic;
            }
        }
    }

    namespace GamestateNumberIdentities
    {
        public class Selector : GamestateNumberIdentity
        {
            public INumberSelector numberSelector;
            public GamestateManyNumbersIdentity numbersIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                numbersIdentity.Initialize(restrictionContext);
            }

            protected override int NumberLogic
                => numberSelector.Apply(numbersIdentity.Numbers);
        }
    }
}