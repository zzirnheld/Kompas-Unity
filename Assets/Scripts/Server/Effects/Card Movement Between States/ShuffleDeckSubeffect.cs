using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class ShuffleDeckSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Player.deckCtrl.Shuffle();
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}