using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class CardFitsRestriction : TriggerRestrictionBase
	{
		public IRestriction<GameCardBase> cardRestriction;
		public IIdentity<GameCardBase> card;
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);
			cardRestriction.Initialize(initializationContext);

			if (AllNull(card, anyOf)) throw new System.ArgumentException($"No card to check against restriction in {initializationContext.effect}");
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