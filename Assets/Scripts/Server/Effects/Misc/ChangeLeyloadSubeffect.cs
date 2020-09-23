using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ChangeLeyloadSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerGame.Leyload += Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}