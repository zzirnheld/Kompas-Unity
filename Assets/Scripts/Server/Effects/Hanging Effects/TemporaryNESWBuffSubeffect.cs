using KompasCore.Cards;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuffSubeffect : TemporaryCardChangeSubeffect
    {
        public int nModifier = 0;
        public int eModifier = 0;
        public int sModifier = 0;
        public int wModifier = 0;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            Debug.Log($"Creating temp NESW buff effect during context {Effect.CurrActivationContext}");
            var temp = new TemporaryNESWBuff(game: ServerGame,
                                             triggerRestriction: triggerRestriction,
                                             endCondition: endCondition,
                                             fallOffCondition: fallOffCondition,
                                             fallOffRestriction: CreateFallOffRestriction(Target),
                                             currentContext: Effect.CurrActivationContext,
                                             buffRecipient: Target,
                                             nBuff: nModifier + Effect.X * nMultiplier,
                                             eBuff: eModifier + Effect.X * eMultiplier,
                                             sBuff: sModifier + Effect.X * sMultiplier,
                                             wBuff: wModifier + Effect.X * wMultiplier);

            return new List<HangingEffect>() { temp };
        }
    }
}