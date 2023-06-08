using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.Exceptions
{
	public class InvalidSpaceException : KompasException
	{
		public readonly Space space;

		public InvalidSpaceException(Space space, string debugMessage = "", string message = "")
			: base(debugMessage, message)
		{
			this.space = space;
		}
	}
}