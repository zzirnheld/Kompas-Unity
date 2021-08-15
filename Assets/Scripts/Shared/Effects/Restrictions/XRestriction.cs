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
        public const string LTEConstant = "<=Constant";
        public const string EqualsConstant = "==Constant";
        public const string LessThanEqualControllerPips = "<= Controller's Pips";
        public const string LessThanEffectX = "<X";
        public const string EqualsEffectX = "=X";
        public const string LessThanEqualEffectX = "<=X";

        public string[] xRestrictions;

        public int constant;

        public GameCard Source { get; private set; }
        public Subeffect Subeffect { get; private set; }

        public void Initialize(GameCard source, Subeffect subeffect = null)
        {
            Source = source;
            Subeffect = subeffect;
        }

        private bool RestrictionValid(string r, int x)
        {
            switch (r)
            {
                case Positive: return x > 0;
                case Negative: return x < 0;
                case Nonnegative: return x >= 0;
                case LessThanEqualThisCost: return x <= Source.Cost;
                case LessThanEqualThisE: return x <= Source.E;
                case LessThanEqualControllerPips: return x <= Source.Controller.Pips;
                case AtLeastConstant: return x >= constant;
                case LTEConstant: return x <= constant;
                case EqualsConstant: return x == constant;
                case LessThanEffectX: return x < Subeffect.Count;
                case EqualsEffectX: return x == Subeffect.Count;
                case LessThanEqualEffectX: return x <= Subeffect.Count;
                default: throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction.");
            }
        }

        public bool Evaluate(int x) => xRestrictions.All(r => RestrictionValid(r, x));
    }
}