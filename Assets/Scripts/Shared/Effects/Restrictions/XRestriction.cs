﻿using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects
{
    [System.Serializable]
    public class XRestriction
    {
        public const string LessThanEqualThisCost = "<=ThisCost";
        public const string LessThanEqualThisE = "<=ThisE";
        public const string Positive = ">0";
        public const string Negative = "<0";
        public const string Nonnegative = ">=0";

        public const string AtLeastConstant = ">=Constant";
        public const string NoMoreThanConstant = "<=Constant";
        public const string EqualsConstant = "==Constant";
        public const string LessThanEqualControllerPips = "<= Controller's Pips";

        public const string LessThanEffectX = "<X";
        public const string GreaterThanEffectX = ">X";
        public const string EqualsEffectX = "=X";
        public const string LessThanEqualEffectX = "<=X";

        public const string LessThanAvatarCardValue = "<Avatar";
        public const string GreaterThanAvatarCardValue = ">Avatar";

        public string[] xRestrictions;

        public int constant;

        public CardValue avatarCardValue;

        public GameCard Source { get; private set; }
        public Subeffect Subeffect { get; private set; }

        public void Initialize(GameCard source, Subeffect subeffect = null)
        {
            Source = source;
            Subeffect = subeffect;
        }

        private bool RestrictionValid(string r, int x)
        {
            return r switch
            {
                Positive => x > 0,
                Negative => x < 0,
                Nonnegative => x >= 0,

                LessThanEqualThisCost       => x <= Source.Cost,
                LessThanEqualThisE          => x <= Source.E,
                LessThanEqualControllerPips => x <= Source.Controller.Pips,

                AtLeastConstant     => x >= constant,
                NoMoreThanConstant  => x <= constant,
                EqualsConstant      => x == constant,

                LessThanEffectX      => x < Subeffect.Count,
                GreaterThanEffectX   => x > Subeffect.Count,
                EqualsEffectX        => x == Subeffect.Count,
                LessThanEqualEffectX => x <= Subeffect.Count,

                LessThanAvatarCardValue => x < avatarCardValue.GetValueOf(Source.Controller.Avatar);
                GreaterThanAvatarCardValue => x > avatarCardValue.GetValueOf(Source.Controller.Avatar);

                _ => throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction."),
            };
        }

        public bool Evaluate(int x) => xRestrictions.All(r => RestrictionValid(r, x));
    }
}