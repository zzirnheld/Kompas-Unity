using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class KompasException : Exception
    {
        public string message;

        public KompasException(string debugMessage, string message)
            : base(debugMessage)
        {
            this.message = message;
        }
    }
}