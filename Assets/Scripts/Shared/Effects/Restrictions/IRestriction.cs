using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public interface IRestriction<Type> : IContextInitializeable
    {
        bool IsValid(Type item, IResolutionContext context);
    }
    
    public abstract class RestrictionBase<Type> : ContextInitializeableBase, IRestriction<Type>
    {
        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            try
            {
                return item != null && IsValidLogic(item, context);
            }
            catch (SystemException exception)
                when (exception is NullReferenceException || exception is ArgumentException)
            {
                Debug.LogError(exception);
                return false;
            }
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }

    public abstract class AllOfBase<Type> : RestrictionBase<Type>
    {
        public IList<IRestriction<Type>> elements = new IRestriction<Type>[] { };

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach (var element in elements) element.Initialize(initializationContext);
            if (elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
        }

        protected override bool IsValidLogic(Type item, IResolutionContext context)
            => elements.All(r => r.IsValid(item, context));
    }
}