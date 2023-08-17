using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class Name : CardRestrictionElement
	{
		[JsonProperty]
		public string nameIs;
		[JsonProperty]
		public string nameIncludes;

		[JsonProperty]
		public IIdentity<GameCardBase> sameAs;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			sameAs.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
		{
			if (nameIs != null && card.CardName != nameIs) return false;
			if (nameIncludes != null && !card.CardName.Contains(nameIncludes)) return false;
			if (sameAs != null && card.CardName != sameAs.From(context).CardName) return false;

			return true;
		}
	}

	public class DistinctName : CardRestrictionElement
	{
		public IIdentity<GameCardBase> from = new Identities.Cards.ThisCardNow();
		public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			from.Initialize(initializationContext);
			cards?.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
		{
			if (cards == default) return from.From(context).CardName != card.CardName;

			return cards.From(context)
				.Select(c => c.CardName)
				.All(name => name != card.CardName);
		}
	}

	public class Unique : CardRestrictionElement
	{
		protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
			=> item.Unique;
	}
}