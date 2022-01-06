﻿using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DelaySubeffect : TemporaryCardChangeSubeffect
    {
        public int numTimesToDelay = 0;
        public bool clearWhenResume = true;
        public override bool ContinueResolution => false;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            Debug.Log($"Is context null? {Context == null}");
            Debug.Log($"Are jump indices null? {jumpIndices == null}");
            Context?.SetResumeInfo(JumpIndex, Effect.CardTargets, Effect.SpaceTargets);
            Context?.Targets?.Clear();
            Context?.Targets?.AddRange(Effect.CardTargets);
            var delay = new DelayedHangingEffect(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(Source),
                                                 sourceEff: Effect,
                                                 currentContext: Context,
                                                 numTimesToDelay: numTimesToDelay,
                                                 toResume: ServerEffect,
                                                 indexToResumeResolution: JumpIndex,
                                                 controller: EffectController,
                                                 targets: new List<GameCard>(Effect.CardTargets),
                                                 spaces: new List<Space>(Effect.SpaceTargets),
                                                 clearIfResolve: clearWhenResume);
            return new List<HangingEffect>() { delay };
        }
    }
}