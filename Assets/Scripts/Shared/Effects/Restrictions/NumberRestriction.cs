using KompasCore.Cards;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class NumberRestriction
    {
        public const string Positive = ">0";
        public const string Negative = "<0";
        public const string Nonnegative = ">=0";

        public const string AtLeastConstant = ">=Constant";
        public const string NoMoreThanConstant = "<=Constant";
        public const string EqualsConstant = "==Constant";

        public const string LessThanEffectX = "<X";
        public const string GreaterThanEffectX = ">X";
        public const string EqualsEffectX = "=X";
        public const string LessThanEqualEffectX = "<=X";

        public const string LessThanEffectArg = "<Arg";

        public const string LessThanAvatarCardValue = "<Avatar";
        public const string GreaterThanAvatarCardValue = ">Avatar";

        public const string GreaterThanSourceCardValue = ">Source";

        public string[] numberRestrictions;

        public int constant;

        public CardValue cardValue;

        public RestrictionContext RestrictionContext { get; private set; }
        public GameCard Source => RestrictionContext.source;
        public Subeffect Subeffect => RestrictionContext.subeffect;

        private bool initialized = false;

        public void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;
            initialized = true;
        }

        private bool IsRestrictionValid(string r, int x) => r switch
        {
            Positive => x > 0,
            Negative => x < 0,
            Nonnegative => x >= 0,

            AtLeastConstant => x >= constant,
            NoMoreThanConstant => x <= constant,
            EqualsConstant => x == constant,

            LessThanEffectX => x < Subeffect.Count,
            GreaterThanEffectX => x > Subeffect.Count,
            EqualsEffectX => x == Subeffect.Count,
            LessThanEqualEffectX => x <= Subeffect.Count,

            LessThanEffectArg => x < Subeffect.Effect.arg,

            LessThanAvatarCardValue => x < cardValue.GetValueOf(Source.Controller.Avatar),
            GreaterThanAvatarCardValue => x > cardValue.GetValueOf(Source.Controller.Avatar),

            _ => throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction."),
        };

        private bool IsRestrictionValidDebug(string r, int x)
        {
            bool answer = IsRestrictionValid(r, x);
            if (!answer) Debug.Log($"{x} flouts {r} when effect count is {Subeffect?.Count}");
            return answer;
        }

        public bool IsValidNumber(int x)
        {
            if (!initialized) throw new System.ArgumentException("X restriction not initialized!");
            return numberRestrictions.All(r => IsRestrictionValid(r, x));
        }
    }
}