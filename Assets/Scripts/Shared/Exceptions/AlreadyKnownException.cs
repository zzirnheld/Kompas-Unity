using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class AlreadyKnownException : KompasException
    {
        public readonly GameCard card;

        public AlreadyKnownException(GameCard card, string debugMessage = "", string message = "")
            : base(debugMessage, message)
        {
            this.card = card;
        }
    }
}