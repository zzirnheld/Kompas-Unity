﻿using KompasCore.Effects.Identities.Cards;

namespace KompasServer.Effects.Subeffects
{
	public class TargetThis : AutoTargetCardIdentity
	{
		public override void Initialize(ServerEffect eff, int subeffIndex)
		{
			subeffectCardIdentity = new ThisCardNow();
			base.Initialize(eff, subeffIndex);
		}
	}
}