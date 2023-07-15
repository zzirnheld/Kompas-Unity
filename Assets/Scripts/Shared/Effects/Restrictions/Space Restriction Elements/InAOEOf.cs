using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class InAOEOf : SpaceRestrictionElement
	{
		public IIdentity<GameCardBase> card;
		public IRestriction<GameCardBase> cardRestriction; //Used to restrict anyOf. If non-null, but anyOf is null, will make anyOf default to All()
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
		public IIdentity<IReadOnlyCollection<GameCardBase>> allOf;

		public IIdentity<int> minAnyOfCount = Identities.Numbers.Constant.One;

		public IIdentity<Space> alsoInAOE;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			if (cardRestriction != null && anyOf == null) anyOf = new Identities.ManyCards.All();

			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			cardRestriction?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);
			allOf?.Initialize(initializationContext);

			if (AllNull(card, cardRestriction, anyOf, allOf))
				throw new System.ArgumentNullException("card", $"Provided no card/s to be in AOE of for {initializationContext.source?.CardName}");

			minAnyOfCount.Initialize(initializationContext);

			alsoInAOE?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var isValidCard = IsValidAOE(space, context);
			if (card != null && !ValidateCard(isValidCard, context)) return false;
			if (anyOf != null && !ValidateAnyOf(isValidCard, context)) return false;
			if (allOf != null && !ValidateAllOf(isValidCard, context)) return false;
			return true;
		}

		private Func<GameCardBase, bool> IsValidAOE(Space space, IResolutionContext context)
		{
			var alsoInAOE = this.alsoInAOE?.From(context);
			if (alsoInAOE == null) return card => card.SpaceInAOE(space);
			else return card => card.SpaceInAOE(space) && card.SpaceInAOE(alsoInAOE);
		}

		private bool ValidateCard(Func<GameCardBase, bool> IsValidCard, IResolutionContext context)
			=> IsValidCard(card.From(context));

		private bool ValidateAnyOf(Func<GameCardBase, bool> IsValidCard, IResolutionContext context) 
		{
			IEnumerable<GameCardBase> cards = anyOf.From(context);
			if (cardRestriction != null) cards = cards.Where(c => cardRestriction.IsValid(c, context));

			return minAnyOfCount.From(context) <= cards.Count(IsValidCard);
		}

		private bool ValidateAllOf(Func<GameCardBase, bool> IsValidCard, IResolutionContext context)
			=> allOf.From(context).All(IsValidCard);
	}
}