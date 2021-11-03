using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class CardNotHereException : KompasException
    {
        public readonly CardLocation location;

        public CardNotHereException(CardLocation location, string message = "")
            : base(message)
        {
            this.location = location;
        }
    }
}