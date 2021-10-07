using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class DelaySubeffect : TemporaryCardChangeSubeffect
    {
        public int numTimesToDelay = 0;
        public int indexToResume;
        public bool clearWhenResume = true;
        public override bool ContinueResolution => false;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            Debug.Log($"Is context null? {Context == null}");
            Context?.Targets?.Clear();
            Context?.Targets?.AddRange(Effect.Targets);
            var delay = new DelayedHangingEffect(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(Source),
                                                 currentContext: Context,
                                                 numTimesToDelay: numTimesToDelay,
                                                 toResume: ServerEffect,
                                                 indexToResumeResolution: indexToResume,
                                                 controller: EffectController,
                                                 targets: new List<GameCard>(Effect.Targets),
                                                 clearIfResolve: clearWhenResume);
            return new List<HangingEffect>() { delay };
        }
    }
}