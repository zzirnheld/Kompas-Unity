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
        public IList<IRestrictionElement<Type>> elements;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach (var element in elements) element.Initialize(initializationContext);
        }

        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            try
            {
                return IsValidLogic(item, context);
            }
            catch (SystemException exception) when (exception is NullReferenceException || exception is ArgumentException)
            {
                Debug.LogError(exception);
                return false;
            }
        }

        protected virtual bool IsValidLogic(Type item, IResolutionContext context)
            => elements.All(r => r.IsValid(item, context));
    }

    public interface IRestrictionElement<Type> : IContextInitializeable
    {
        public bool IsValid(Type item, IResolutionContext context);
    }

    public abstract class RestrictionElementBase<Type> : ContextInitializeableBase, IRestrictionElement<Type>
    {
        public bool IsValid(Type item, IResolutionContext context)
        {
            ComplainIfNotInitialized();

            return item != null && IsValidLogic(item, context);
        }

        protected abstract bool IsValidLogic(Type item, IResolutionContext context);
    }
}