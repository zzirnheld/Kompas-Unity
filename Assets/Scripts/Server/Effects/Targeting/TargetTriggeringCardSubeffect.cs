using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Context.BeforeCardInfo == null) throw new NullCardException(NoValidCardTarget);

            ServerEffect.AddTarget(Context.BeforeCardInfo.Card);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}