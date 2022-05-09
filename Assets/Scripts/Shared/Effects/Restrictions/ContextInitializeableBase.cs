using System;

namespace KompasCore.Effects
{
    public abstract class ContextInitializeableBase
    {
        protected bool Initialized { get; private set; }

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            Initialized = true;
        }

        protected void ComplainIfNotInitialized()
        {
            if (!Initialized) throw new NotImplementedException($"You forgot to initialize a {GetType()}!");
        }
    }

    public interface IContextInitializeable
    {
        public void Initialize(RestrictionContext restrictionContext);
    }
}