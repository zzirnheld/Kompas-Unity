namespace KompasCore.Effects.Identities
{
    /// <summary>
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
    /// </summary>
    public abstract class GamestateNumberIdentityBase : ContextInitializeableBase,
        INoActivationContextIdentity<int>, IActivationContextIdentity<int>
    {
        protected abstract int AbstractNumber { get; }

        public int Item
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractNumber;
            }
        }

        public int From(ActivationContext context, ActivationContext secondaryContext) => Item;
    }

    namespace GamestateNumberIdentities
    {
        public class Constant : GamestateNumberIdentityBase
        {
            public static Constant ONE => new Constant { constant = 1 };

            public int constant;

            protected override int AbstractNumber => constant;
        }
    }
}