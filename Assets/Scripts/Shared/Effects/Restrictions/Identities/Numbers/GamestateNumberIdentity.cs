namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextNumberIdentity : IContextInitializeable
    {
        public int Number { get; }
    }

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