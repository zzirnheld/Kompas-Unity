namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextNumberIdentity : IContextInitializeable
    {
        public int Number { get; }
    }

    /// <summary>
    /// Can be used whether or not the caller does or doesn't care about an ActivationContext.
    /// (Eventually, can be used whether or not the caller does or doesn't care about an ActivationContext,
    /// but that interface doesn't even exist yet.)
    /// </summary>
    public abstract class GamestateNumberIdentityBase : ContextInitializeableBase,
        INoActivationContextNumberIdentity
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
    { }
}