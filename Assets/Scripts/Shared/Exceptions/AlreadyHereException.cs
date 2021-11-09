using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KompasCore.Exceptions
{
    public class AlreadyHereException : KompasException
    {
        public readonly CardLocation location;

        public AlreadyHereException(CardLocation location, string debugMessage = "", string message = "")
            : base(debugMessage, message)
        {
            this.location = location;
        }
    }
}
