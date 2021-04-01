using KompasCore.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Effects
{
    [Serializable]
    public class CardValue
    {
        #region values
        private const string SummoningCost = "S";
        #endregion values

        public string value;
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public int GetValueOf(IGameCardInfo card)
        {
            if (card == null) throw new ArgumentException("Cannot get value of null card", "card");

            switch (value)
            {
                case SummoningCost: return card.S;
                default: throw new ArgumentException($"Invalid value string {value}", "value");
            }
        }
    }
}