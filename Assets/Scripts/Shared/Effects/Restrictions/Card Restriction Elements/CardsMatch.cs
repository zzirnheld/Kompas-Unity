using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class CardsMatch : CardRestrictionElement
	{
		[JsonProperty]
		public IIdentity<GameCardBase> card;
		[JsonProperty]
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			card?.Initialize(initializationContext);
			cards?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
		{
			if (card != null) return item?.Card == card.From(context).Card;
			else return cards.From(context).Any(c => c.Card == item?.Card);
		}
	}
}