﻿using KompasCore.Effects.Identities.ActivationContextNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class SetTurnsOnBoard : SetCardStats
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            turnsOnBoard = new X() { multiplier = xMultiplier, modifier = xModifier, divisor = xDivisor };
            base.Initialize(eff, subeffIndex);
        }
    }
}