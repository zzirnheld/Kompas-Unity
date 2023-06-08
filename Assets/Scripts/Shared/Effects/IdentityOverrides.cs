
using System;
using System.Collections.Generic;
using KompasCore.Cards;

namespace KompasCore.Effects
{
	public class IdentityOverrides
	{
		private readonly Stack<GameCardBase> targetOverrides = new Stack<GameCardBase>();
		public GameCardBase TargetCardOverride => targetOverrides.Count > 0 ? targetOverrides.Peek() : null;

		public T WithTargetCardOverride<T> (GameCardBase targetOverride, Func<T> action)
		{
			targetOverrides.Push(targetOverride);
			var t = action();
			targetOverrides.Pop();
			return t;
		}
	}
}