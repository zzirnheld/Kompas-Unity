﻿using KompasCore.Effects.Restrictions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
	public class ConditionalJump : ServerSubeffect
	{
		public IGamestateRestriction jumpIfTrue;

		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			base.Initialize(eff, subeffIndex);
			jumpIfTrue.Initialize(DefaultInitializationContext);
		}

		public override Task<ResolutionInfo> Resolve()
		{
			if (jumpIfTrue.IsValid(ResolutionContext)) return Task.FromResult(ResolutionInfo.Index(JumpIndex));
			else return Task.FromResult(ResolutionInfo.Next);
		}
	}
}