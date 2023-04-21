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

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var element in elements) element.Initialize(initializationContext);
			if (elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context)
			=> elements.All(r => r.IsValid(item, context));
	}
}