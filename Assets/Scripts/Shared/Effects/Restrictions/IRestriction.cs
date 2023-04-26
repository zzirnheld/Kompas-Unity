using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
	public interface IRestriction<RestrictedType> : IContextInitializeable
	{
		bool IsValid(RestrictedType item, IResolutionContext context);
	}
	
	public abstract class RestrictionBase<RestrictedType> : ContextInitializeableBase, IRestriction<RestrictedType>
	{
		public bool IsValid(RestrictedType item, IResolutionContext context)
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

		protected abstract bool IsValidLogic(RestrictedType item, IResolutionContext context);
	}

	public abstract class AllOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		public IList<IRestriction<RestrictedType>> elements = new IRestriction<RestrictedType>[] { };

		protected virtual bool LogSoloElements => true;

        public delegate bool ShouldIgnore(IRestriction<RestrictedType> restriction);
        private ShouldIgnore ignorePredicate = elem => false;

		public bool IsValidIgnoring(RestrictedType item, IResolutionContext context, ShouldIgnore ignorePredicate)
		{
            ShouldIgnore oldVal = this.ignorePredicate;
            this.ignorePredicate = ignorePredicate;
            bool ret = IsValid(item, context);
            this.ignorePredicate = oldVal;
            return ret;
        }

        public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var element in elements) element.Initialize(initializationContext);
			if (LogSoloElements && elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context) => elements
			.Where(r => !ignorePredicate(r))
			.All(r => r.IsValid(item, context));
	}
}