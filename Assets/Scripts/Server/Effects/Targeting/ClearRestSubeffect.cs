using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class ClearRestSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.Rest.Clear();
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}