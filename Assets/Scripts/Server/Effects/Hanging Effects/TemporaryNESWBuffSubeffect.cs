using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuffSubeffect : TemporaryCardChangeSubeffect
    {
        public int nBuff = 0;
        public int eBuff = 0;
        public int sBuff = 0;
        public int wBuff = 0;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            var temp = new TemporaryNESWBuff(game: ServerGame,
                                             triggerRestriction: triggerRestriction,
                                             endCondition: endCondition,
                                             fallOffCondition: fallOffCondition,
                                             fallOffRestriction: CreateFallOffRestriction(Target),
                                             currentContext: Effect.CurrActivationContext,
                                             buffRecipient: Target,
                                             nBuff: nBuff + Effect.X * nMultiplier,
                                             eBuff: eBuff + Effect.X * eMultiplier,
                                             sBuff: sBuff + Effect.X * sMultiplier,
                                             wBuff: wBuff + Effect.X * wMultiplier);

            return new List<HangingEffect>() { temp };
        }
    }
}