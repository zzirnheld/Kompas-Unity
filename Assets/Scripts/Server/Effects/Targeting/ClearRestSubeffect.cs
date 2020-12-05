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
            Effect.rest.Clear();
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}