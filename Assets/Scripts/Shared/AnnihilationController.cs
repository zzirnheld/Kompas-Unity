﻿using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class AnnihilationController : MonoBehaviour, IGameLocation
    {
        public Game game;
        public Player owner;

        public CardLocation CardLocation => CardLocation.Annihilation;
        public List<GameCard> Cards { get; } = new List<GameCard>();

        public virtual void Annihilate(GameCard card, IStackable stackSrc = null)
        {
            if (card.GameLocation == this) throw new AlreadyHereException(CardLocation.Annihilation);

            card.Remove(stackSrc);
            Cards.Add(card);
            card.GameLocation = this;
            SpreadOutCards();
        }

        public virtual void Remove(GameCard card)
        {
            if (!Cards.Contains(card)) throw new CardNotHereException(CardLocation.Annihilation);

            Cards.Remove(card);
            SpreadOutCards();
        }

        public void SpreadOutCards()
        {
            float spreadOutMultipler = 2f * (owner.index == 0 ? -1f : 1f);
            int max = Cards.Count - 1;

            //iterate through children, set the z coord
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = new Vector3(spreadOutMultipler * (float)(max - i), 0, 0);
            }
        }
    }
}