using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class MillSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            for(int i = 0; i < Count; i++) Controller.deckCtrl.Topdeck.Discard();

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}