using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class CardsMatch : CardRestrictionElement
	{
		public IIdentity<GameCardBase> card;
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
		{
			if (card != null) return item?.Card == card.From(context, default).Card;
			else return cards.From(context, default).Any(c => c.Card == item?.Card);
		}
	}
}