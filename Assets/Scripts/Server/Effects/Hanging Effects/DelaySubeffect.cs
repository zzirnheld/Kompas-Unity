using KompasCore.Cards;
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
            Context?.SetResumeInfo(JumpIndex, Effect.Targets, Effect.Coords);
            Context?.Targets?.Clear();
            Context?.Targets?.AddRange(Effect.Targets);
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
                                                 targets: new List<GameCard>(Effect.Targets),
                                                 spaces: new List<Space>(Effect.Coords),
                                                 clearIfResolve: clearWhenResume);
            return new List<HangingEffect>() { delay };
        }
    }
}