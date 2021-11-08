using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.CardInfo == null) throw new NullCardException(NoValidCardTarget);

            ServerEffect.AddTarget(Context.CardInfo.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}