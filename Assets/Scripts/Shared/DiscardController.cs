﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using KompasCore.UI;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
	public abstract class DiscardController : OwnedGameLocation
	{
		public DiscardUIController discardUIController;

		protected readonly IList<GameCard> discard = new List<GameCard>();

		public override CardLocation CardLocation => CardLocation.Discard;
		public override IEnumerable<GameCard> Cards => discard;

		public override void Refresh() => discardUIController.Refresh();

		//adding/removing cards
		public virtual bool Discard(GameCard card, IStackable stackSrc = null)
		{
			if (card == null) throw new NullCardException("Cannot add null card to discard");
			if (Equals(card.GameLocation)) throw new AlreadyHereException(CardLocation.Discard);

			//Check if the card is successfully removed (if it's not, it's probably an avatar)
			bool successful = card.Remove(stackSrc);
			if (successful)
			{
				Debug.Log($"Discarding {card}");
				discard.Add(card);
				card.Controller = Owner;
				card.GameLocation = this;
				card.Position = null;
				discardUIController.Refresh();
			}
			else Debug.LogWarning($"Failed to discard {card}");
			return successful;
		}

		public override void Remove(GameCard card)
		{
			if (!discard.Contains(card)) throw new CardNotHereException(CardLocation.Discard, card);

			discard.Remove(card);
			discardUIController.Refresh();
		}

		public override int IndexOf(GameCard card) => discard.IndexOf(card);
	}
}