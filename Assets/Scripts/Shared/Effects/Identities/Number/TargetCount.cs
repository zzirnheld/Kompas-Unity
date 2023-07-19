using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities.Numbers
{

	public class TargetCount : EffectContextualLeafIdentityBase<int>
	{
		public IRestriction<GameCardBase> cardRestriction = new Restrictions.GamestateRestrictionElements.AlwaysValid();

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override int AbstractItemFrom(IResolutionContext toConsider)
			=> InitializationContext.subeffect.Effect.CardTargets
				.Count(c => cardRestriction.IsValid(c, toConsider));
	}
}