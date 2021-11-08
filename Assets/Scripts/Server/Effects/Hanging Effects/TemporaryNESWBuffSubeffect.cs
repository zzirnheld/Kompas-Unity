using KompasCore.Exceptions;
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
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, ChangedStatsOfCardOffBoard);

            Debug.Log($"Creating temp NESW buff effect during context {Context}");
            var temp = new TemporaryNESWBuff(game: ServerGame,
                                             triggerRestriction: triggerRestriction,
                                             endCondition: endCondition,
                                             fallOffCondition: fallOffCondition,
                                             fallOffRestriction: CreateFallOffRestriction(Target),
                                             sourceEff: Effect,
                                             currentContext: Context,
                                             buffRecipient: Target,
                                             nBuff: nModifier + Effect.X * nMultiplier,
                                             eBuff: eModifier + Effect.X * eMultiplier,
                                             sBuff: sModifier + Effect.X * sMultiplier,
                                             wBuff: wModifier + Effect.X * wMultiplier);

            return new List<HangingEffect>() { temp };
        }
    }
}