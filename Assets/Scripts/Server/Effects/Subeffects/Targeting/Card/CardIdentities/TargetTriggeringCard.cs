﻿using KompasCore.Exceptions;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class TargetTriggeringCard : ServerSubeffect
    {
        public bool secondary = false;
        public bool info = false;
        public bool cause = false;

        public override Task<ResolutionInfo> Resolve()
        {
            var cardInfoToTarget = ResolutionContext.TriggerContext.mainCardInfoBefore;
            if (secondary) cardInfoToTarget = ResolutionContext.TriggerContext.secondaryCardInfoBefore;
            if (cause) cardInfoToTarget = ResolutionContext.TriggerContext.cardCauseBefore;
            if (cardInfoToTarget == null) throw new NullCardException(NoValidCardTarget);

            if (info) ServerEffect.cardInfoTargets.Add(cardInfoToTarget);
            else ServerEffect.AddTarget(cardInfoToTarget.Card);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}