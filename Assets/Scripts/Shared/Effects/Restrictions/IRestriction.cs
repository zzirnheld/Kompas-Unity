using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Helpers;
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

	public interface IAllOf<RestrictedType> : IRestriction<RestrictedType>
	{
		public bool IsValidIgnoring(RestrictedType item, IResolutionContext context, AllOfBase<RestrictedType>.ShouldIgnore ignorePredicate);
	}

	public abstract class AllOfBase<RestrictedType> : RestrictionBase<RestrictedType>, IAllOf<RestrictedType>
	{
		public IList<IRestriction<RestrictedType>> elements = new IRestriction<RestrictedType>[] { };

		protected virtual bool LogSoloElements => true;
		protected virtual IEnumerable<IRestriction<RestrictedType>> DefaultElements => Enumerable.Empty<IRestriction<RestrictedType>>();

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
			elements = elements.Safe()
				.Concat(DefaultElements)
				.ToList();
			base.Initialize(initializationContext);
			foreach (var element in elements) element.Initialize(initializationContext);
			if (LogSoloElements && elements.Count == 1) Debug.LogWarning($"only one element on {GetType()} on eff of {initializationContext.source}");
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context) => elements
			.Where(r => !ignorePredicate(r))
			.All(r => r.IsValid(item, context));
	}

	public abstract class AnyOfBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		public IRestriction<RestrictedType>[] restrictions;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			foreach (var restriction in restrictions) restriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context)
			=> restrictions.Any(r => r.IsValid(item, context));
	}

	public abstract class DualRestrictionBase<RestrictedType> : RestrictionBase<RestrictedType>
	{
		protected abstract IEnumerable<IRestriction<RestrictedType>> DefaultRestrictions { get; }

		/// <summary>
		/// A piece of a movement restriction.
		/// Basically, a movement restriction is made up of two groups of restrictions -
		/// one that's checked for a normal move (i.e. player-initiated during an open gamestate),
		/// and one that's checked when the card moves by effect
		/// </summary>
		protected class DualComponentRestriction : AllOfBase<RestrictedType>
		{

			private readonly IReadOnlyList<IRestriction<RestrictedType>> restrictions;

			/// <summary>
			/// Constructs a new piece of an overall MovementRestriction
			/// </summary>
			/// <param name="sharedRestrictions">Restrictions that are shared among all elements of the MovementRestriction.
			/// If this is null, the DefaultMovementRestrictions are used instead.</param>
			/// <param name="specificRestrictions">Restrictions that are specific to this particular ComponentMovementRestriction</param>
			public DualComponentRestriction(IEnumerable<IRestriction<RestrictedType>> sharedRestrictions,
				IEnumerable<IRestriction<RestrictedType>> defaultRestrictions,
				params IEnumerable<IRestriction<RestrictedType>>[] specificRestrictions)
			{
				var interimRestrictions = sharedRestrictions ?? defaultRestrictions;
				foreach(var additionalRestrictions in specificRestrictions)
				{
					interimRestrictions = interimRestrictions.Concat(additionalRestrictions);
				}
				this.restrictions = interimRestrictions.ToArray();
			}

			//In this case, "Default" is checked when Initialize is called. since this gets constructed before it's initialized, we're good.
			protected override IEnumerable<IRestriction<RestrictedType>> DefaultElements => restrictions;
		}

		protected DualComponentRestriction NormalRestriction { get; private set; }
		protected DualComponentRestriction EffectRestriction { get; private set; }

		public IRestriction<RestrictedType>[] normalAndEffect = null;
		public IRestriction<RestrictedType>[] normalOnly = new IRestriction<RestrictedType>[] { };
		public IRestriction<RestrictedType>[] effectOnly = new IRestriction<RestrictedType>[] { };

		/// <summary>
		/// Restrictions that, by default, apply to a player moving a card normally (but not by effect)
		/// </summary>
		protected abstract IEnumerable<IRestriction<RestrictedType>> DefaultNormalRestrictions { get; }

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			
			NormalRestriction = new DualComponentRestriction(sharedRestrictions: normalAndEffect, defaultRestrictions: DefaultRestrictions,
				DefaultNormalRestrictions, normalOnly);
			NormalRestriction.Initialize(initializationContext);

			EffectRestriction = new DualComponentRestriction(sharedRestrictions: normalAndEffect, defaultRestrictions: DefaultRestrictions,
				effectOnly);
			EffectRestriction.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(RestrictedType item, IResolutionContext context)
			=> context.TriggerContext.stackableCause == null
				? NormalRestriction.IsValid(item, context)
				: EffectRestriction.IsValid(item, context);
	}
}