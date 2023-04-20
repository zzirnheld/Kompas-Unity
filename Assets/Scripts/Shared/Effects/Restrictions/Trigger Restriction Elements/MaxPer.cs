using UnityEngine;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public abstract class MaxPer : TriggerRestrictionElement
    {
        public int max = 1;

        protected int Max => max; //Futureproofing in case I want to allow an identity instead, for "x times per turn"

        protected abstract int Uses { get; }

        protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context)
            => Uses < Max; // ? true : LogFalse();

        //private bool LogFalse() { Debug.Log($"{Uses} exceeded {max} in {InitializationContext.effect}"); return false; }
    }

    public class MaxPerTurn : MaxPer
    {
        protected override int Uses => InitializationContext.effect.TimesUsedThisTurn;
    }

    public class MaxPerRound : MaxPer
    {
        protected override int Uses => InitializationContext.effect.TimesUsedThisRound;
    }

    public class MaxPerStack : MaxPer
    {
        protected override int Uses => InitializationContext.effect.TimesUsedThisStack;
    }
}