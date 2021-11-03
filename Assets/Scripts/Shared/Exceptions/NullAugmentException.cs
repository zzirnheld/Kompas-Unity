using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class NullAugmentException : KompasException
    {
        public readonly IStackable augmentSrc;
        public readonly GameCard augmentedCard;

        public NullAugmentException (IStackable augmentSrc, GameCard augmentedCard, string message = "")
            : base(message)
        {
            this.augmentSrc = augmentSrc;
            this.augmentedCard = augmentedCard;
        }
    }
}