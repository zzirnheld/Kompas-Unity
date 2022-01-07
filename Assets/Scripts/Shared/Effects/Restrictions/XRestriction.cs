using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects
{
    public class XRestriction
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

        public string[] xRestrictions;

        public int constant;

        public CardValue avatarCardValue;

        public GameCard Source { get; private set; }
        public Subeffect Subeffect { get; private set; }

        public void Initialize(GameCard source, Subeffect subeffect = null)
        {
            Source = source;
            Subeffect = subeffect;
            avatarCardValue?.Initialize(Source);
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

            LessThanAvatarCardValue => x < avatarCardValue.GetValueOf(Source.Controller.Avatar),
            GreaterThanAvatarCardValue => x > avatarCardValue.GetValueOf(Source.Controller.Avatar),

            _ => throw new System.ArgumentException($"Invalid X restriction {r} in X Restriction."),
        };

        public bool IsValidNumber(int x) => xRestrictions.All(r => IsRestrictionValid(r, x));
    }
}