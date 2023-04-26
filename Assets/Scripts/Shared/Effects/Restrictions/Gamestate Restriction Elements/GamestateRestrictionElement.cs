
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
	/// <summary>
	/// Exists on its own as a version of a "restriction" that doesn't require any context
	/// </summary>
	public interface IGamestateRestriction : IContextInitializeable
	{
		bool IsValid(IResolutionContext context);
	}

	//TODO make this accept context by default, and have another intermediate base class that gets rid of the context (so can pass context to child restrictions)
	public abstract class GamestateRestrictionBase : ContextInitializeableBase, IGamestateRestriction, IRestriction<TriggeringEventContext>, IRestriction<Player>
	{
		public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid(context);
		public bool IsValid(Player item, IResolutionContext context) => IsValid(context);

		public bool IsValid(IResolutionContext context)
		{
			ComplainIfNotInitialized();

			try
			{
				return IsValidLogic(context);
			}
			catch (SystemException exception)
				when (exception is NullReferenceException || exception is ArgumentException)
			{
				Debug.LogError(exception);
				return false;
			}
		}

		protected abstract bool IsValidLogic(IResolutionContext context);
	}

	namespace GamestateRestrictionElements
	{
		public class AllOf : GamestateRestrictionBase
		{
			public IList<IGamestateRestriction> elements = new IGamestateRestriction[] { };

			protected virtual bool LogSoloElements => true;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				foreach (var element in elements) element.Initialize(initializationContext);
				if (LogSoloElements && elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
			}

			protected override bool IsValidLogic(IResolutionContext context)
				=> elements.All(r => r.IsValid(context));
		}
	}
}