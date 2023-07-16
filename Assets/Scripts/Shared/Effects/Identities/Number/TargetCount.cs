using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities.Numbers
{

	public class TargetCount : ContextualParentIdentityBase<int>
	{
		public IRestriction<GameCardBase> cardRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			cardRestriction?.Initialize(initializationContext);
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction?.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		private System.Func<GameCardBase, bool> Selector(IResolutionContext context)
			=> card => cardRestriction?.IsValid(card, context) ?? true;

		protected override int AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> InitializationContext.subeffect.Effect.CardTargets.Count(Selector(ContextToConsider(context, secondaryContext)));
	}
}