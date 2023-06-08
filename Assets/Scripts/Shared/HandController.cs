using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.GameCore
{
	public abstract class HandController : OwnedGameLocation
	{
		public override CardLocation CardLocation => CardLocation.Hand;
		public override IEnumerable<GameCard> Cards => hand;

		protected readonly List<GameCard> hand = new List<GameCard>();

		public GameObject leftDummy;
		public GameObject rightDummy;

		public int HandSize => hand.Count;
		public override int IndexOf(GameCard card) => hand.IndexOf(card);

		public virtual bool Hand(GameCard card, IStackable stackSrc = null)
		{
			if (card == null) throw new NullCardException("Cannot add null card to hand");
			if (Equals(card.GameLocation)) throw new AlreadyHereException(CardLocation.Hand);

			var successful = card.Remove(stackSrc);
			if (successful)
			{
				Debug.Log($"Handing {card.CardName}");

				hand.Add(card);
				card.GameLocation = this;
				card.Position = null;
				card.Controller = Owner; //TODO should this be before or after the prev line?

				SpreadOutCards();
			}
			return successful;
		}

		public override void Remove(GameCard card)
		{
			if (!hand.Contains(card)) throw new CardNotHereException(CardLocation, card,
				$"Hand of \n{string.Join(", ", hand.Select(c => c.CardName))}\n doesn't contain {card}, can't remove it!");

			hand.Remove(card);
			SpreadOutCards();
		}

		public override void Refresh() => SpreadOutCards();

		public void SpreadOutCards()
		{
			//leftDummy.transform.localPosition = new Vector3(2.25f * (((float)hand.Count / -2f) + -1f + 0.5f), 0, 0);
			//rightDummy.transform.localPosition = new Vector3(2.25f * (((float)hand.Count / -2f) + (float)hand.Count + 0.5f), 0, 0);
			//iterate through children, set the z coord
			for (int i = 0; i < hand.Count; i++)
			{
				hand[i].CardController.transform.parent = transform;
				float offset = ((float)hand.Count / -2f) + (float)i + 0.5f;
				hand[i].CardController.transform.localPosition = new Vector3(2.25f * offset, 0, 0);
				hand[i].CardController.SetRotation();
				hand[i].CardController.gameObject.SetActive(true);
			}
		}
	}
}