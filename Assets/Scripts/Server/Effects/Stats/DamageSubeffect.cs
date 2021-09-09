using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DamageSubeffect : ServerSubeffect
    {
        public override bool IsImpossible() => Target == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));

            Target.TakeDamage(Count, Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}