﻿using KompasCore.Cards;
using System;
using System.Linq;

namespace KompasCore.Effects
{
    public class CardValue
    {
        #region values
        private const string Nimbleness = "N";
        private const string Endurance = "E";
        private const string SummoningCost = "S";
        private const string Wounding = "W";
        private const string CastingCost = "C";
        private const string AugmentCost = "A";

        private const string Cost = "Cost";
        private const string NumberOfAugments = "Number of Augments";
        #endregion values

        public string value;
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public int GetValueOf(IGameCardInfo card)
        {
            if (card == null) throw new ArgumentException("Cannot get value of null card", "card");

            return value switch
            {
                Nimbleness       => card.N,
                Endurance        => card.E,
                SummoningCost    => card.S,
                Wounding         => card.W,
                CastingCost      => card.C,
                AugmentCost      => card.A,

                Cost             => card.Cost,
                NumberOfAugments => card.Augments.Count(),
                _ => throw new ArgumentException($"Invalid value string {value}", "value"),
            };
        }

        public void SetValueOf(GameCard card, int num, IStackable stackSrc = null)
        {
            if (card == null) throw new ArgumentException("Cannot set value of null card", "card");

            switch (value)
            {
                case Nimbleness: 
                    card.SetN(num, stackSrc: stackSrc);
                    break;
                case Endurance:
                    card.SetE(num, stackSrc: stackSrc);
                    break;
                case SummoningCost:
                    card.SetS(num, stackSrc: stackSrc);
                    break;
                case Wounding:
                    card.SetW(num, stackSrc: stackSrc);
                    break;
                case CastingCost:
                    card.SetC(num, stackSrc: stackSrc);
                    break;
                case AugmentCost:
                    card.SetA(num, stackSrc: stackSrc);
                    break;
            }
        }
    }
}