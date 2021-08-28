using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.CardInfo == null)
            {
                Debug.LogError($"(see: tiwaz bug) Unable to target triggering card because it's null. Activation context: {Context}");
                return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            }

            ServerEffect.AddTarget(Context.CardInfo.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}