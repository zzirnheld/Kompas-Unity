using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ServerEffect.CurrActivationContext.CardInfo == null)
            {
                Debug.LogError($"(see: tiwaz bug) Unable to target triggering card because it's null. Activation context: {ServerEffect.CurrActivationContext}");
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            }

            ServerEffect.AddTarget(ServerEffect.CurrActivationContext.CardInfo.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}