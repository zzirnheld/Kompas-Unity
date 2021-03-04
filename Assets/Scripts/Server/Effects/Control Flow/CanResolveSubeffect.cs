using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class CanResolveSubeffect : ServerSubeffect
    {
        public int[] subeffIndices;
        private IEnumerable<ServerSubeffect> Subeffects => subeffIndices.Select(s => ServerEffect.subeffects[s]);

        public int skipIndex = -1;

        public override Task<ResolutionInfo> Resolve()
        {
            var impossible = Subeffects.FirstOrDefault(s => s.IsImpossible());
            if (impossible == default) return Task.FromResult(ResolutionInfo.Next);
            else
            {
                if(skipIndex == -1) return Task.FromResult(ResolutionInfo.Impossible($"{impossible} could not have resolved."));
                else return Task.FromResult(ResolutionInfo.Index(skipIndex));
            }
        }
    }
}