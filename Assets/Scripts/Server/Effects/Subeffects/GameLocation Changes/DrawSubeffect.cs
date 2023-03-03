﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.Draw(PlayerTarget);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}