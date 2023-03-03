﻿using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ShuffleDeckSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            PlayerTarget.deckCtrl.Shuffle();
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}