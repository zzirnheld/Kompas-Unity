﻿using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AttackSubeffect : ServerSubeffect
    {
        public int attackerIndex = -2;

        public override Task<ResolutionInfo> Resolve()
        {
            var attacker = Effect.GetTarget(attackerIndex);
            var defender = Target;
            if (attacker == null)
                throw new NullCardException("Attacker was null");
            else if (defender == null)
                throw new NullCardException("Defender was null");

            ServerGame.Attack(attacker, defender, ServerEffect.ServerController);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}