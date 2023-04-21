using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using UnityEngine;

namespace KompasCore.Effects
{
    public interface IRestriction<Type> : IContextInitializeable
    {
        bool IsValid(Type item, IResolutionContext context);
    }

    public class RestrictionBase<Type> : ContextInitializeableBase, IRestriction<Type>
    {
        public IList<IRestriction<Type>> elements = new IRestriction<Type>[] { };

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            foreach (var element in elements) element.Initialize(initializationContext);
            if (elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
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
}