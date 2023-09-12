using KompasCore.Cards;
using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Cards
{

	public class AugmentedCard : ContextualParentIdentityBase<GameCardBase>
	{
		[JsonProperty]
		public IIdentity<GameCardBase> ofThisCard;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			ofThisCard.Initialize(initializationContext);
		}

		protected override GameCardBase AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
			=> ofThisCard.From(context, secondaryContext).AugmentedCard;
	}
}