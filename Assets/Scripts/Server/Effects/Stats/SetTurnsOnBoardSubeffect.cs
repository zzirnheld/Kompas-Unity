using KompasServer.Effects;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class SetTurnsOnBoardSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Location != CardLocation.Field)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            Target.SetTurnsOnBoard(Count);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}