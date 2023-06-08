
using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
	/// <summary>
	/// Exists on its own as a version of a "restriction" that doesn't require any context
	/// </summary>
	public interface IGamestateRestriction : IContextInitializeable,
		IRestriction<TriggeringEventContext>, IRestriction<Player>, IRestriction<GameCardBase>
	{
		bool IsValid(IResolutionContext context);
	}

	public abstract class GamestateRestrictionBase : ContextInitializeableBase, IGamestateRestriction
	{
		public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid(context);
		public bool IsValid(Player item, IResolutionContext context) => IsValid(context);
		public bool IsValid(GameCardBase item, IResolutionContext context) => IsValid(context);

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

		public class Not : GamestateRestrictionBase
		{
			public IGamestateRestriction negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(IResolutionContext context)
				=> !negated.IsValid(context);
		}

		//TODO deduplicate with TriggerRestrictionElements.CardFitsRestriction
		public class CardFitsRestriction : GamestateRestrictionBase
		{
			public IRestriction<GameCardBase> cardRestriction;
			public IIdentity<GameCardBase> card;
			public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				card?.Initialize(initializationContext);
				anyOf?.Initialize(initializationContext);
				cardRestriction.Initialize(initializationContext);

				if (AllNull(card, anyOf)) throw new System.ArgumentException($"No card to check against restriction in {initializationContext.effect}");
			}

			protected override bool IsValidLogic(IResolutionContext contextToConsider)
			{
				bool IsValidCard(GameCardBase c) => !cardRestriction.IsValid(c, contextToConsider);

				if (card != null && !IsValidCard(card.From(contextToConsider))) return false;
				if (anyOf != null && !anyOf.From(contextToConsider).Any(IsValidCard)) return false;

				return true;
			}
		}
	}
}