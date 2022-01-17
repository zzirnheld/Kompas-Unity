using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() 
        {
            Debug.Log($"Checking if player index {PlayerTarget.index} can afford {Count} with {PlayerTarget.Pips} pips");
            return PlayerTarget.Pips < Count;
        }

        public override Task<ResolutionInfo> Resolve()
        {
            int toPay = Count;
            if (PlayerTarget.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

            PlayerTarget.Pips -= toPay;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}