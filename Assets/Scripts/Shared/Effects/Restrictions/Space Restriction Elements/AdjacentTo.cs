using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	/// <summary>
	/// Simplifies the adjacency case, even though it could just be done with "compare distance to 1".
	/// </summary>
	public class AdjacentTo : SpaceRestrictionElement
	{
		public IRestriction<GameCardBase> cardRestriction;
		public IIdentity<int> cardRestrictionMinimum = Identities.Numbers.Constant.One;
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOfTheseCards;
		public IIdentity<GameCardBase> card;
		public IIdentity<Space> space;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction?.Initialize(initializationContext);
			anyOfTheseCards?.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			space?.Initialize(initializationContext);
			if (AllNull(card, space, anyOfTheseCards, cardRestriction))
				throw new System.NotImplementedException($"Forgot to provide a space or card to be adjacent to " +
					$"in the effect of {InitializationContext.source}");
			else if (MultipleNonNull(card, space, anyOfTheseCards, cardRestriction))
				throw new System.NotImplementedException($"Provided both a space and a card to be adjacent to " +
					$"in the effect of {InitializationContext.source}");
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(Space toTest, IResolutionContext context)
		{
			if (cardRestriction != null)
				return toTest.AdjacentSpaces
					.Select(InitializationContext.game.BoardController.GetCardAt)
					.Count(c => cardRestriction.IsValid(c, context))
					>= cardRestrictionMinimum.From(context);
			else if (anyOfTheseCards != null)
				return anyOfTheseCards
					.From(context)
					.Any(c => c.IsAdjacentTo(toTest));
			else if (card != null)
				return card
					.From(context)
					.IsAdjacentTo(toTest);
			else if (space != null)
				return space
					.From(context)
					.IsAdjacentTo(toTest);
			else throw new System.NotImplementedException($"You forgot to account for some weird case for {InitializationContext.source}");
		}
	}
}