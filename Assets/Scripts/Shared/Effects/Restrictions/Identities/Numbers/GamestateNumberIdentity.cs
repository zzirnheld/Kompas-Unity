using KompasServer.Effects.Identities;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateNumberIdentity : ContextInitializeableBase,
        ISubeffectNumberIdentity
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