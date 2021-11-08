using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class NullCardException : KompasException
    {
        public NullCardException (string debugMessage, string message = "")
            : base(debugMessage, message)
        { }
    }
}