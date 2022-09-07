using System;
using UnityEngine;

namespace KompasCore.Effects
{
    public interface IContextInitializeable
    {
        public void Initialize(EffectInitializationContext initializationContext);
    }

    public abstract class ContextInitializeableBase : IContextInitializeable
    {
        protected bool Initialized { get; private set; }

        protected EffectInitializationContext InitializationContext { get; private set; }

        public virtual void Initialize(EffectInitializationContext initializationContext)
        {
            if (Initialized)
            {
                Debug.Log($"Was already initialized with {InitializationContext}, but now being initialized with {initializationContext}");
            }
            InitializationContext = initializationContext;

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
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            if (initializationContext.subeffect == null) throw new ArgumentNullException($"{GetType()} must be initialized by/with a Subeffect");
            base.Initialize(initializationContext);
        }
    }
}