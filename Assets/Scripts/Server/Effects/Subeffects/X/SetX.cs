﻿using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
	public class SetX : ServerSubeffect
	{
		public virtual int BaseCount => Effect.X;

		public int TrueCount => (BaseCount * xMultiplier / xDivisor) + xModifier + (change ? Effect.X : 0);

		public bool change = false;

		public override Task<ResolutionInfo> Resolve()
		{
			Effect.X = TrueCount;
			Debug.Log($"Setting X to {Effect.X}");
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}