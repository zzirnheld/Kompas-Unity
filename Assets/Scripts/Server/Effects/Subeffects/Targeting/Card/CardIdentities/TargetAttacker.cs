﻿using KompasCore.Effects.Identities.ActivationContextCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAttacker : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new Attacker();
            base.Initialize(eff, subeffIndex);
        }
    }
}