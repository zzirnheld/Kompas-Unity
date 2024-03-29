﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	/// <summary>
	/// Swaps two values among one card's own NESW. E for W, for example.
	/// </summary>
	public class SwapOwnNESW : ServerSubeffect
	{
		public int Stat1;
		public int Stat2;

		public override Task<ResolutionInfo> Resolve()
		{
			if (CardTarget == null)
				throw new NullCardException(TargetWasNull);
			else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
				throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

			int[] newStats = { CardTarget.N, CardTarget.E, CardTarget.S, CardTarget.W };
			(newStats[Stat1], newStats[Stat2]) = (newStats[Stat2], newStats[Stat1]);
			CardTarget.SetCharStats(newStats[0], newStats[1], newStats[2], newStats[3]);

			return Task.FromResult(ResolutionInfo.Next);
		}
	}
}