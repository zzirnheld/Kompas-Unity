﻿using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class HangingAnnihilationSubeffect : HangingEffectSubeffect
    {
        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var contextCopy = CurrentContext.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);
            var eff = new HangingAnnihilationEffect(serverGame: ServerGame,
                                                    triggerRestriction: triggerRestriction,
                                                    endCondition: endCondition,
                                                    fallOffCondition: fallOffCondition,
                                                    fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                                    sourceEff: Effect,
                                                    currentContext: contextCopy,
                                                    target: CardTarget);
            return new List<HangingEffect>() { eff };
        }
    }
}