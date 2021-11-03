using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
    public class InvalidSpaceException : KompasException
    {
        public readonly Space space;

        public InvalidSpaceException(Space space, string message = "")
            : base(message)
        {
            this.space = space;
        }
    }
}