using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;

namespace KompasCore.Effects.Restrictions
{
	public abstract class TriggerRestrictionBase : RestrictionBase<TriggeringEventContext>
	{
		public bool useDummyResolutionContext = true;

		protected virtual IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
			=> useDummyResolutionContext
				? IResolutionContext.Dummy(triggeringContext)
				: resolutionContext;
	}

	/// <summary>
	/// Base class for trigger restrictions that can also act like gamestate restrictions
	/// </summary>
	public abstract class TriggerGamestateRestrictionBase : TriggerRestrictionBase, IGamestateRestriction
	{
		public bool IsValid(IResolutionContext context) => IsValid(context.TriggerContext, context);

		public bool IsValid(int item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(Space item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(Player item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(GameCardBase item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid((Space s, Player p) item, IResolutionContext context) => IsValid(context.TriggerContext, context);
		public bool IsValid(IEnumerable<GameCardBase> item, IResolutionContext context) => IsValid(context.TriggerContext, context);

		//Fulfill IListRestriction contract
		public bool AllowsValidChoice(IEnumerable<GameCardBase> options, IResolutionContext context) => true;
		public IEnumerable<GameCardBase> Deduplicate(IEnumerable<GameCardBase> options) => options;
		public int GetMinimum(IResolutionContext context) => 0;
		public int GetMaximum(IResolutionContext context) => int.MaxValue;
		public bool IsValidClientSide(IEnumerable<GameCardBase> options, IResolutionContext context) => IsValid(options, context);
		public void PrepareForSending(IResolutionContext context) { }
	}

	namespace TriggerRestrictionElements
	{
		public class AllOf : AllOfBase<TriggeringEventContext>
		{
			protected override bool LogSoloElements => false;

			public static readonly ISet<Type> ReevalationRestrictions = new HashSet<Type>(new Type[] {
				typeof(GamestateRestrictionElements.MaxPerTurn),
				typeof(GamestateRestrictionElements.MaxPerRound),
				typeof(GamestateRestrictionElements.MaxPerStack)
			});

			public static readonly IRestriction<TriggeringEventContext>[] DefaultFallOffRestrictions = {
				new TriggerRestrictionElements.CardsMatch(){
					card = new Identities.Cards.ThisCardNow(),
					other = new Identities.Cards.CardBefore()
				},
				new TriggerRestrictionElements.ThisCardInPlay() };

			/// <summary>
			/// Reevaluates the trigger to check that any restrictions that could change between it being triggered
			/// and it being ordered on the stack, are still true.
			/// (Not relevant to delayed things, since those expire after a given number of uses (if at all), so yeah
			/// </summary>
			/// <returns></returns>
			public bool IsStillValidTriggeringContext(TriggeringEventContext context)
				=> elements.Where(elem => ReevalationRestrictions.Contains(elem.GetType()))
						.All(elem => elem.IsValid(context, default));
		}

		public class AlwaysValid : TriggerGamestateRestrictionBase
		{
			protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context) => true;
		}

		public class NeverValid : TriggerGamestateRestrictionBase
		{
			protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context) => false;
		}

		public class ThisCardInPlay : TriggerGamestateRestrictionBase
		{
			protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
				=> InitializationContext.source.Location == CardLocation.Board;
		}

		public class NothingHappening : TriggerGamestateRestrictionBase
		{
			protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context)
				=> InitializationContext.game.NothingHappening;
		}

		public class FriendlyTurn : TriggerGamestateRestrictionBase
		{
			protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context)
				=> InitializationContext.game.TurnPlayer == InitializationContext.Controller;
		}

		public class AnyOf : TriggerRestrictionBase
		{
			public IRestriction<TriggeringEventContext> [] restrictions;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				foreach (var r in restrictions) r.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
				=> restrictions.Any(r => r.IsValid(context, secondaryContext));
		}

		public class Not : TriggerRestrictionBase
		{
			public IRestriction<TriggeringEventContext>  inverted;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				inverted.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
				=> !inverted.IsValid(context, secondaryContext);
		}
	}
}