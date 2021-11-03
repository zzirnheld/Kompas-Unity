using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class KompasException : Exception
    {
        public KompasException(string message)
            : base(message)
        { }
    }
}