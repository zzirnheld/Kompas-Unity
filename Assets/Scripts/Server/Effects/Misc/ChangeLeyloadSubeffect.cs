using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ChangeLeyloadSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Leyload += Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}