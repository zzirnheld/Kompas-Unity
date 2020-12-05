using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class MillSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            for (int i = 0; i < Count; i++)
            {
                var card = Player.deckCtrl.Topdeck;
                if (card == null) return ServerEffect.EffectImpossible();
                card.Discard();
            }

            return ServerEffect.ResolveNextSubeffect();
        }
    }
}