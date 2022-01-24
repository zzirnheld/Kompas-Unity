using KompasCore.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TemporaryNESWBuffSubeffect : HangingEffectSubeffect
    {
        public int nModifier = 0;
        public int eModifier = 0;
        public int sModifier = 0;
        public int wModifier = 0;
        public int cModifier = 0;
        public int aModifier = 0;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;
        public int cMultiplier = 0;
        public int aMultiplier = 0;

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            Debug.Log($"Creating temp NESW buff effect during context {Context}");
            var temp = new TemporaryNESWBuff(game: ServerGame,
                                             triggerRestriction: triggerRestriction,
                                             endCondition: endCondition,
                                             fallOffCondition: fallOffCondition,
                                             fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                             sourceEff: Effect,
                                             currentContext: Context,
                                             buffRecipient: CardTarget,
                                             nBuff: nModifier + Effect.X * nMultiplier,
                                             eBuff: eModifier + Effect.X * eMultiplier,
                                             sBuff: sModifier + Effect.X * sMultiplier,
                                             wBuff: wModifier + Effect.X * wMultiplier,
                                             cBuff: cModifier + Effect.X * cMultiplier,
                                             aBuff: aModifier + Effect.X * aMultiplier);

            return new List<HangingEffect>() { temp };
        }
    }
}