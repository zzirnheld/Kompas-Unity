using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class InvalidLocationException : KompasException
    {
        public readonly CardLocation location;

        public InvalidLocationException(CardLocation location, string message = "")
            : base(message)
        {
            this.location = location;
        }
    }
}