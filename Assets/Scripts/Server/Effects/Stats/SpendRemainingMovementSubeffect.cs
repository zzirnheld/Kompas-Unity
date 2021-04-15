using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class SpendRemainingMovementSubeffect : ServerSubeffect
    {
        public int mult = 1;
        public int div = 1;
        public int mod = 0;

        public override Task<ResolutionInfo> Resolve()
        {
            int toSpend = (Target.SpacesCanMove * mult / div) + mod;
            if (toSpend <= 0 || Target.SpacesCanMove < toSpend) return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));

            int toSet = Target.SpacesMoved + toSpend;
            Target.SetSpacesMoved(toSet);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}