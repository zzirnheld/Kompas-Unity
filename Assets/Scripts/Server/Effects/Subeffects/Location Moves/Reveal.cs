﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class Reveal : ServerSubeffect
	{
		public override bool IsImpossible(TargetingContext overrideContext = null)
			=> GetCardTarget(overrideContext)?.KnownToEnemy != false; //account for null prop

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null) throw new NullCardException(TargetWasNull);

			CardTarget.Reveal(Effect);
			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}