using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
	public abstract class SpaceRestrictionElement : RestrictionBase<Space>, IRestriction<GameCardBase>, IRestriction<TriggeringEventContext>, IRestriction<(Space s, Player p)>
	{
		public bool IsValid(GameCardBase item, IResolutionContext context) => IsValid(item?.Position, context);
		public bool IsValid(TriggeringEventContext item, IResolutionContext context) => IsValid(item?.space, context);
		public bool IsValid((Space s, Player p) item, IResolutionContext context) => IsValid(item.s, context);
	}

	namespace SpaceRestrictionElements
	{

		public class AllOf : AllOfBase<Space> { }

		public class Not : SpaceRestrictionElement
		{
			public IRestriction<Space> negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Space space, IResolutionContext context)
				=> !negated.IsValid(space, context);
		}

		public class AnyOf : SpaceRestrictionElement
		{
			public IRestriction<Space>[] restrictions;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				foreach (var r in restrictions) r.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Space space, IResolutionContext context)
				=> restrictions.Any(r => r.IsValid(space, context));
		}

		public class Empty : SpaceRestrictionElement
		{
			protected override bool IsValidLogic(Space space, IResolutionContext context)
				=> InitializationContext.game.BoardController.IsEmpty(space);
		}

		public class Different : SpaceRestrictionElement
		{
			public IIdentity<Space> from;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				from.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(Space item, IResolutionContext context)
				=> from.From(context) != item;
		}

		/// <summary>
		/// Spell rule: you can't place a spell where it would block a path, through spaces that don't contain a spell, between the avatars
		/// So to be valid, a card has to either not be a spell, or it has to be a valid place to put a spell.
		/// </summary>
		public class SpellRule : SpaceRestrictionElement
		{
			protected override bool IsValidLogic(Space item, IResolutionContext context)
				=> InitializationContext.source.CardType != 'S'
				|| InitializationContext.game.BoardController.ValidSpellSpaceFor(InitializationContext.source, item);
		}

		public class CardFitsRestriction : SpaceRestrictionElement
		{
			public IRestriction<GameCardBase> restriction;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				restriction.Initialize(initializationContext);
			}

			public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
			{
				base.AdjustSubeffectIndices(increment, startingAtIndex);
				restriction.AdjustSubeffectIndices(increment, startingAtIndex);
			}

			protected override bool IsValidLogic(Space space, IResolutionContext context)
			{
				var card = InitializationContext.game.BoardController.GetCardAt(space);
				return restriction.IsValid(card, context);
			}
		}
	}
}