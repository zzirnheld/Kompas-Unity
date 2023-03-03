using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffect
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public bool secondary = false;
        public bool info = false;
        public bool cause = false;

        public override Task<ResolutionInfo> Resolve()
        {
            var cardInfoToTarget = CurrentContext.mainCardInfoBefore;
            if (secondary) cardInfoToTarget = CurrentContext.secondaryCardInfoBefore;
            if (cause) cardInfoToTarget = CurrentContext.cardCauseBefore;
            if (cardInfoToTarget == null) throw new NullCardException(NoValidCardTarget);

            if (info) ServerEffect.cardInfoTargets.Add(cardInfoToTarget);
            else ServerEffect.AddTarget(cardInfoToTarget.Card);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}