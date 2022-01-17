﻿using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SpendRemainingMovementSubeffect : ServerSubeffect
    {
        public int mult = 1;
        public int div = 1;
        public int mod = 0;

        public override Task<ResolutionInfo> Resolve()
        {
            int toSpend = (CardTarget.SpacesCanMove * mult / div) + mod;
            if (toSpend <= 0 || CardTarget.SpacesCanMove < toSpend) return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

            int toSet = CardTarget.SpacesMoved + toSpend;
            CardTarget.SetSpacesMoved(toSet);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}