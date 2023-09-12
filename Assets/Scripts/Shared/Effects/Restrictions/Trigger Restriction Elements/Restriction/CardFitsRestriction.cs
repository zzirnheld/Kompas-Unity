using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class CardFitsRestriction : TriggerGamestateRestrictionBase
	{
		[JsonProperty]
		public IIdentity<GameCardBase> card;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;
		
		[JsonProperty(Required = Required.Always)]
		public IRestriction<GameCardBase> cardRestriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);

			if (AllNull(card, anyOf)) throw new System.ArgumentException($"No card to check against restriction in {initializationContext.effect}");
		}

		public override void AdjustSubeffectIndices(int increment, int startingAtIndex = 0)
		{
			base.AdjustSubeffectIndices(increment, startingAtIndex);
			cardRestriction.AdjustSubeffectIndices(increment, startingAtIndex);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			var contextToConsider = ContextToConsider(context, secondaryContext);
			bool IsValidCard(GameCardBase c) => !cardRestriction.IsValid(c, contextToConsider);

			if (card != null && !IsValidCard(FromIdentity(card, context, secondaryContext))) return false;
			if (anyOf != null && !FromIdentity(anyOf, context, secondaryContext).Any(IsValidCard)) return false;

			return true;
		}

		protected virtual IdentityType FromIdentity<IdentityType>
			(IIdentity<IdentityType> identity, TriggeringEventContext triggeringEventContext, IResolutionContext resolutionContext)
			=> identity.From(triggeringEventContext, resolutionContext);
	}
}