using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class MillSubeffect : ServerSubeffect
    {
        public int xMultiplier = 0;
        public int xDivisor = 1;
        public int modifier = 0;

        public override bool Resolve()
        {
            var num = (Effect.X * xMultiplier / xDivisor) + modifier;
            for(int i = 0; i < num; i++) Controller.deckCtrl.Topdeck.Discard();

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}