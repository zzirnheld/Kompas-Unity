using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public class MillSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            for (int i = 0; i < Count; i++)
            {
                var card = Player.deckCtrl.Topdeck;
                if (card == null) return Task.FromResult(ResolutionInfo.Impossible(CouldntMillAllX));
                ServerEffect.AddTarget(card);
                card.Discard(ServerEffect);
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}