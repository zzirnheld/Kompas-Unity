using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class CardsMatch : TriggerRestrictionBase
	{
		public IIdentity<GameCardBase> card;
		public IIdentity<GameCardBase> other;
		public IIdentity<IReadOnlyCollection<GameCardBase>> anyOf;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card.Initialize(initializationContext);

			other?.Initialize(initializationContext);
			anyOf?.Initialize(initializationContext);

			if (AllNull(other, anyOf)) throw new System.ArgumentNullException("other", "No card to compare the card to in trigger restriction element");
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			var first = card.From(context, secondaryContext)?.Card;

			if (other != null && first != other.From(context, secondaryContext)?.Card) return false;
			if (anyOf != null && !anyOf.From(context, secondaryContext).Any(c => c?.Card == first)) return false;

			return true;
		}
	}
}