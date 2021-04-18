using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class PayPipsSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() 
        {
            Debug.Log($"Checking if player index {Player.Index} can afford {Count} with {Player.Pips} pips");
            return Player.Pips < Count;
        }

        public override Task<ResolutionInfo> Resolve()
        {
            int toPay = Count;
            if (Player.Pips < toPay) return Task.FromResult(ResolutionInfo.Impossible(CantAffordPips));

            Player.Pips -= toPay;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}