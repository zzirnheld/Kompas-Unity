using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class NullCardException : KompasException
    {
        public NullCardException (string message)
            : base(message)
        { }
    }
}