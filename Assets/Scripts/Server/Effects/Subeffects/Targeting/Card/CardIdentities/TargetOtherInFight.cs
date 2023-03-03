﻿using KompasCore.Cards;
using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetOtherInFight : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (CurrentContext.stackableCause is KompasCore.Effects.Attack attack)
            {
                GameCard newTarget = null;
                if (attack.attacker == CardTarget) newTarget = attack.defender;
                else if (attack.defender == CardTarget) newTarget = attack.attacker;

                if (newTarget != null)
                {
                    ServerEffect.AddTarget(newTarget);
                    return Task.FromResult(ResolutionInfo.Next);
                }
            }

            return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));
        }
    }
}