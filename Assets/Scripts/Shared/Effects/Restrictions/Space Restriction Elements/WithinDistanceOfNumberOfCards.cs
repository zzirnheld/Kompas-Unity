using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Numbers;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class WithinDistanceOfNumberOfCards : SpaceRestrictionElement
	{
		public IRestriction<GameCardBase> cardRestriction = new GamestateRestrictionElements.AlwaysValid();

		public IIdentity<int> numberOfCards = Constant.One;
		public IIdentity<int> distance = Constant.One;

		public bool excludeSelf = true;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
			numberOfCards.Initialize(initializationContext);
			distance.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			return InitializationContext.game.Cards
				.Where(c => c.DistanceTo(space) < distance.From(context))
				.Where(c => cardRestriction.IsValid(c, context))
				.Count() >= numberOfCards.From(context);
		}
	}
}