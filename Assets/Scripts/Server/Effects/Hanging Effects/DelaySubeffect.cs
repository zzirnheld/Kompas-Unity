using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class DelaySubeffect : TemporaryCardChangeSubeffect
    {
        public int numTimesToDelay = 0;
        public int indexToResume;
        public override bool ContinueResolution => false;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var delay = new DelayedHangingEffect(game: ServerGame,
                                                 triggerRestriction: triggerRestriction,
                                                 endCondition: endCondition,
                                                 fallOffCondition: fallOffCondition,
                                                 fallOffRestriction: CreateFallOffRestriction(Source),
                                                 currentContext: Effect.CurrActivationContext,
                                                 numTimesToDelay: numTimesToDelay,
                                                 toResume: ServerEffect,
                                                 indexToResumeResolution: indexToResume,
                                                 controller: EffectController,
                                                 targets: new List<GameCard>(Effect.Targets));
            return new List<HangingEffect>() { delay };
        }
    }
}