using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ChangeLeyloadSubeffect : ServerSubeffect
    {
        public int multiplier = 0;
        public int divisor = 1;
        public int modifier = 0;

        public override bool Resolve()
        {
            ServerGame.Leyload += (Effect.X * multiplier / divisor) + modifier;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}