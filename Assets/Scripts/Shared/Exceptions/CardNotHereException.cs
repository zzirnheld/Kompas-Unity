using KompasCore.Cards;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class CardNotHereException : KompasException
    {
        public readonly CardLocation location;
        public readonly GameCardBase card;

        public CardNotHereException(CardLocation location, GameCardBase card, string debugMessage = "", string message = "")
            : base(debugMessage, message)
        {
            this.location = location;
        }
    }
}