﻿using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AddRestSubeffect : CardTargetSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.rest.AddRange(ServerGame.Cards.Where(c => cardRestriction.Evaluate(c, Context)));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}