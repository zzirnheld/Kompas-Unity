using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManyCards
{
	public class Hand : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = false;

		protected override IReadOnlyCollection<GameCardBase> AbstractItem
		{
			get
			{
				var cards = new List<GameCardBase>();
				if (friendly) cards.AddRange(InitializationContext.Controller.handCtrl.Cards);
				if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.handCtrl.Cards);
				return cards;
			}
		}
	}
}