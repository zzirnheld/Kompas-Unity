using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
	public class CardAtPosition : ContextualParentIdentityBase<GameCardBase>
	{
		public IIdentity<Space> position;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			position.Initialize(initializationContext);
		}

		protected override GameCardBase AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
		{
			var finalSpace = position.From(context, secondaryContext);
			return context.TriggerContext.game.BoardController.GetCardAt(finalSpace);
		}
	}
}