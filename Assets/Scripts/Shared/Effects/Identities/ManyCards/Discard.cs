using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace KompasCore.Effects.Identities.ManyCards
{
	public class Discard : ContextlessLeafIdentityBase<IReadOnlyCollection<GameCardBase>>
	{
		[JsonProperty]
		public bool friendly = true;
		[JsonProperty]
		public bool enemy = true;

		protected override IReadOnlyCollection<GameCardBase> AbstractItem
		{
			get
			{
				var cards = new List<GameCardBase>();
				if (friendly) cards.AddRange(InitializationContext.Controller.discardCtrl.Cards);
				if (enemy) cards.AddRange(InitializationContext.Controller.Enemy.discardCtrl.Cards);
				return cards;
			}
		}
	}
}