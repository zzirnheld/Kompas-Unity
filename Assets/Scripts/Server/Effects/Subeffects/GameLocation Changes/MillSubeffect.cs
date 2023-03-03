﻿using KompasCore.Cards.Movement;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class MillSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            for (int i = 0; i < Count; i++)
            {
                var card = PlayerTarget.deckCtrl.Topdeck;
                if (card == null) return Task.FromResult(ResolutionInfo.Impossible(CouldntMillAllX));
                ServerEffect.AddTarget(card);
                card.Discard(ServerEffect);
            }

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}