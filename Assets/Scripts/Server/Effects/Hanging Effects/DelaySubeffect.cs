using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class DelaySubeffect : TemporaryCardChangeSubeffect
    {
        public int numTimesToDelay = 0;
        public int indexToResume;
        public string triggerCondition;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var delay = new DelayedHangingEffect(ServerGame, triggerRestriction, endCondition, 
                fallOffCondition, CreateFallOffRestriction(Source),
                numTimesToDelay, ServerEffect, indexToResume, EffectController, new List<GameCard>(Effect.Targets));
            return new List<HangingEffect>() { delay };
        }
    }
}