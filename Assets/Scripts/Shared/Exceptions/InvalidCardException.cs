using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class InvalidCardException : KompasException
    {
        public readonly GameCard card;

        public InvalidCardException(GameCard card, string message = "")
            : base(message)
        {
            this.card = card;
        }
    }
}