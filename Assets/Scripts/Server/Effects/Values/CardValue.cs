using KompasCore.Cards;
using KompasCore.Exceptions;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace KompasCore.Effects
{
    public class CardValue : ContextInitializeableBase
    {
        #region values
        public const string Nimbleness = "N";
        public const string Endurance = "E";
        public const string SummoningCost = "S";
        public const string Wounding = "W";
        public const string CastingCost = "C";
        public const string AugmentCost = "A";

        public const string Cost = "Cost";
        public const string NumberOfAugments = "Number of Augments";
        public const string DistanceToSource = "Distance to Source";
        #endregion values

        public string value;
        public int multiplier = 1;
        public int divisor = 1;
        public int modifier = 0;

        public GameCard Source => InitializationContext.source;

        public int GetValueOf(GameCardBase card)
        {
            ComplainIfNotInitialized();
            if (card == null) throw new NullCardException("Cannot get value of null card");

            int intermediateValue = value switch
            {
                Nimbleness => card.N,
                Endurance => card.E,
                SummoningCost => card.S,
                Wounding => card.W,
                CastingCost => card.C,
                AugmentCost => card.A,

                Cost => card.Cost,
                NumberOfAugments => card.Augments.Count(),
                DistanceToSource => card.DistanceTo(Source),
                _ => throw new ArgumentException($"Invalid value string {value}", "value"),
            };
            return intermediateValue * multiplier / divisor + modifier;
        }

        public void SetValueOf(GameCard card, int num, IStackable stackSrc = null)
        {
            if (card == null) throw new ArgumentException("Cannot set value of null card", "card");

            int intermediateValue = num * multiplier / divisor + modifier;
            switch (value)
            {
                case Nimbleness:
                    card.SetN(intermediateValue, stackSrc: stackSrc);
                    break;
                case Endurance:
                    card.SetE(intermediateValue, stackSrc: stackSrc);
                    break;
                case SummoningCost:
                    card.SetS(intermediateValue, stackSrc: stackSrc);
                    break;
                case Wounding:
                    card.SetW(intermediateValue, stackSrc: stackSrc);
                    break;
                case CastingCost:
                    card.SetC(intermediateValue, stackSrc: stackSrc);
                    break;
                case AugmentCost:
                    card.SetA(intermediateValue, stackSrc: stackSrc);
                    break;
                default:
                    throw new ArgumentException($"Can't set {value} of a card!");
            }
        }
    }
}