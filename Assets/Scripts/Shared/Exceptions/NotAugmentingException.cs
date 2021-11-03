using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class NotAugmentingException : KompasException
    {
        public readonly GameCard card;
        public NotAugmentingException(GameCard card, string message = "")
            : base(message)
        {
            this.card = card;
        }
    }
}