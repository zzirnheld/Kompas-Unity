using KompasCore.Cards;
using KompasCore.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
	public abstract class CardRestrictionElement : RestrictionBase<GameCardBase>, IRestriction<Space>
	{
		public bool IsValid(Space item, IResolutionContext context)
			=> IsValid(InitializationContext.game.BoardController.GetCardAt(item), context);
	}

	namespace CardRestrictionElements
	{
	
		public class AllOf : AllOfBase<GameCardBase>
		{
			public string blurb;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				if (blurb != null) Debug.LogWarning($"{GetType()} blurb is on the card restriction. move it to the card target of {initializationContext.source}");
			}

			public GameCard Source => InitializationContext.source;

			public override string ToString() => $"Card Restriction of {Source?.CardName}." +
				$"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";
		}
		
		public class AlwaysValid : CardRestrictionElement
		{
			protected override bool IsValidLogic(GameCardBase item, IResolutionContext context) => true;
		}

		public class Not : CardRestrictionElement
		{
			public IRestriction<GameCardBase> negated;

			public override void Initialize(EffectInitializationContext initializationContext)
			{
				base.Initialize(initializationContext);
				negated.Initialize(initializationContext);
			}

			protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
				=> !negated.IsValid(item, context);
		}

		public class CardExists : CardRestrictionElement
		{
			protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
				=> card != null;
		}

		/// <summary>
		/// A specialized AllOf containing the default elements, plus 
		/// </summary>
		public class AttackingDefender : AllOf
		{
			protected override IEnumerable<IRestriction<GameCardBase>> DefaultElements
			{
				get
				{
					yield return new Restrictions.GamestateRestrictionElements.CardFitsRestriction()
					{
						card = new Identities.Cards.ThisCardNow(),
						cardRestriction = new AllOf()
						{
							elements = new IRestriction<GameCardBase>[]
							{
								new Friendly(),
								new Character(),
								new Location(CardLocation.Board)
							}
						}
					};
					yield return new Character();
					yield return new Enemy();
					yield return new Restrictions.SpaceRestrictionElements.AdjacentTo()
					{
						card = new Identities.Cards.ThisCardNow()
					};
					yield return new Restrictions.GamestateRestrictionElements.MaxPerTurn();
					yield return new Restrictions.GamestateRestrictionElements.NothingHappening();
				}
			}
		}
	}
}