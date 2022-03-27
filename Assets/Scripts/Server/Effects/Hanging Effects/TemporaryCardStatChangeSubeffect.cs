using KompasCore.Exceptions;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Effects
{
    public class TemporaryCardStatChangeSubeffect : HangingEffectSubeffect
    {
        public int nModifier = 0;
        public int eModifier = 0;
        public int sModifier = 0;
        public int wModifier = 0;
        public int cModifier = 0;
        public int aModifier = 0;

        public int nDivisor = 1;
        public int eDivisor = 1;
        public int sDivisor = 1;
        public int wDivisor = 1;
        public int cDivisor = 1;
        public int aDivisor = 1;

        public int nMultiplier = 0;
        public int eMultiplier = 0;
        public int sMultiplier = 0;
        public int wMultiplier = 0;
        public int cMultiplier = 0;
        public int aMultiplier = 0;

        protected CardStats Buff
        {
            get
            {
                CardStats buff = (nMultiplier, eMultiplier, sMultiplier, wMultiplier, cMultiplier, aMultiplier);
                buff *= Effect.X;
                buff += (nModifier, eModifier, sModifier, wModifier, cModifier, aModifier);
                buff /= (nDivisor, eDivisor, sDivisor, wDivisor, cDivisor, aDivisor);
                return buff;
            }
        }

        protected override IEnumerable<HangingEffect> CreateHangingEffects()
        {
            if (CardTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && CardTarget.Location != CardLocation.Board)
                throw new InvalidLocationException(CardTarget.Location, CardTarget, ChangedStatsOfCardOffBoard);

            Debug.Log($"Creating temp NESW buff effect during context {Context}");
            var contextCopy = Context.Copy;
            contextCopy.SetResumeInfo(Effect.CardTargets, Effect.SpaceTargets, Effect.stackableTargets,
                CardTarget, SpaceTarget, StackableTarget);

            var temp = new TemporaryCardStatChange(game: ServerGame,
                                             triggerRestriction: triggerRestriction,
                                             endCondition: endCondition,
                                             fallOffCondition: fallOffCondition,
                                             fallOffRestriction: CreateFallOffRestriction(CardTarget),
                                             sourceEff: Effect,
                                             currentContext: contextCopy,
                                             buffRecipient: CardTarget,
                                             buff: Buff);

            return new List<HangingEffect>() { temp };
        }
    }
}