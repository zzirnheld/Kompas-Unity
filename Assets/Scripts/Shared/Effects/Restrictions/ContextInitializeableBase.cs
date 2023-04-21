using System;
using System.Collections.Generic;
using System.Linq;
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

        protected virtual IEnumerable<IInitializationRequirement> InitializationRequirements => Enumerable.Empty<IInitializationRequirement>();

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
            if (!Initialized) throw new NotImplementedException($"You forgot to initialize a {GetType()}!\n{this}");
        }
        
        protected static bool AllNull(params object[] objs) => objs.All(o => o == null);
        protected static bool MultipleNonNull(params object[] objs) => objs.Count(o => o != null) > 1;

        public override string ToString()
        {
            return GetType().ToString();
        }
    }

    public interface IInitializationRequirement
    {
        public bool Validate(EffectInitializationContext initializationContext);
    }

    public class SubeffectInitializationRequirement : IInitializationRequirement
    {
        public bool Validate(EffectInitializationContext initializationContext)
        {
            if (initializationContext.subeffect == null) throw new ArgumentNullException($"{GetType()} must be initialized by/with a Subeffect");

            return true;
        }
    }
}