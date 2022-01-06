using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public bool secondary = false;
        public bool info = false;

        public override Task<ResolutionInfo> Resolve()
        {
            var cardInfoToTarget = secondary ? Context.secondaryCardInfoBefore : Context.mainCardInfoBefore;
            if (cardInfoToTarget == null) throw new NullCardException(NoValidCardTarget);

            if (info) ServerEffect.cardInfoTargets.Add(cardInfoToTarget);
            else ServerEffect.AddTarget(cardInfoToTarget.Card);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}