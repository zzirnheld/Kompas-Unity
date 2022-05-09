using System;

namespace KompasCore.Effects
{
    public interface IContextInitializeable
    {
        public void Initialize(RestrictionContext restrictionContext);
    }

    public abstract class ContextInitializeableBase : IContextInitializeable
    {
        protected bool Initialized { get; private set; }

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            Initialized = true;
        }

        protected virtual void ComplainIfNotInitialized()
        {
            if (!Initialized) throw new NotImplementedException($"You forgot to initialize a {GetType()}!");
        }
    }

    /// <summary>
    /// A wrapper inheritor of ContextInitializeableBase that also checks that the restriction context has a subeffect
    /// </summary>
    public abstract class SubeffectInitializeableBase : ContextInitializeableBase
    {
        public override void Initialize(RestrictionContext restrictionContext)
        {
            if (restrictionContext.subeffect == null) throw new ArgumentNullException($"{GetType()} must be initialized by/with a Subeffect");
            base.Initialize(restrictionContext);
        }
    }
}