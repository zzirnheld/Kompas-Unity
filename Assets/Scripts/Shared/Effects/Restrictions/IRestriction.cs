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

    public abstract class RestrictionBase<Type> : ContextInitializeableBase, IRestriction<Type>
    {
        public IList<IRestrictionElement<Type>> elements = new IRestrictionElement<Type>[] { };

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
    
    public class CardRestriction : RestrictionBase<GameCardBase>
    {
        public string blurb = "";

        public GameCard Source => InitializationContext.source;

        public override string ToString() => $"Card Restriction of {Source?.CardName}." +
            $"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";

    }
    
    public class SpaceRestriction : RestrictionBase<Space>
    {

        public string blurb = "";

        //TODO correct any places still using mustBeEmpty

        public Func<Space, bool> AsThroughPredicate(IResolutionContext context)
            => s => IsValid(s, context);

        public Func<Space, bool> IsValidFor(IResolutionContext context) => s => IsValid(s, context);
    }
}