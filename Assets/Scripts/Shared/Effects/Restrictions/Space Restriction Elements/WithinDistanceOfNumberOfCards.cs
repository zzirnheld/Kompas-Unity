using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Numbers;
using Newtonsoft.Json;
using System.Linq;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	public class WithinDistanceOfNumberOfCards : SpaceRestrictionElement
	{
		[JsonProperty]
		public IRestriction<GameCardBase> cardRestriction = new GamestateRestrictionElements.AlwaysValid();

		[JsonProperty]
		public IIdentity<int> numberOfCards = Constant.One;
		[JsonProperty]
		public IIdentity<int> distance = Constant.One;

		[JsonProperty]
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
				.Where(c => !excludeSelf || c != InitializationContext.source)
				.Count() >= numberOfCards.From(context);
		}
	}
}