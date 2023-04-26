
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
        bool IsValid();
    }

    public abstract class GamestateRestrictionBase : ContextInitializeableBase, IGamestateRestriction, IRestriction<TriggeringEventContext>
    {
        public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid();

        public bool IsValid()
        {
			ComplainIfNotInitialized();

			try
			{
				return IsValidLogic();
			}
			catch (SystemException exception)
				when (exception is NullReferenceException || exception is ArgumentException)
			{
				Debug.LogError(exception);
				return false;
			}
        }

        protected abstract bool IsValidLogic();
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

            protected override bool IsValidLogic()
                => elements.All(r => r.IsValid());
		}
    }
}