using System;
using UnityEngine;

namespace KompasCore.Effects
{
	public interface IRestriction<RestrictedType> : IContextInitializeable
	{
		bool IsValid(RestrictedType item, IResolutionContext context);
	}
	
	public abstract class RestrictionBase<RestrictedType> : ContextInitializeableBase, IRestriction<RestrictedType>
	{
		protected virtual bool AllowNullItem => false;

		public bool IsValid(RestrictedType item, IResolutionContext context)
		{
			ComplainIfNotInitialized();

			try
			{
				if (item == null && !AllowNullItem) return false;
				return IsValidLogic(item, context);
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
}