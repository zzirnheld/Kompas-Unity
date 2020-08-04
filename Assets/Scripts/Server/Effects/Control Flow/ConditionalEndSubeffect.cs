using KompasCore.Effects;
using System.Linq;

namespace KompasServer.Effects
{
    public class ConditionalEndSubeffect : ServerSubeffect
    {
        public const string XLessThan0 = "X<0";
        public const string XLessThanEqual0 = "X<=0";
        public const string XGreaterThanConst = "X>C";
        public const string XLessThanConst = "X<C";
        public const string NoneFitRestriction = "None Fit Restriction";
        public const string MustBeFriendlyTurn = "Must be Friendly Turn";
        public const string MustBeEnemyTurn = "Must be Enemy Turn";
        public const string TargetViolatesRestriction = "Target Violates Restriction";
        public const string TargetFitsRestriction = "Target Fits Restriction";

        public int constant = 0;
        public CardRestriction cardRestriction = new CardRestriction();

        public string condition;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }

        private bool ShouldEnd(string condition)
        {
            switch (condition)
            {
                case XLessThan0:         return ServerEffect.X < 0;
                case XLessThanEqual0:    return ServerEffect.X <= 0;
                case XGreaterThanConst:  return ServerEffect.X > constant;
                case XLessThanConst:     return ServerEffect.X < constant;
                case NoneFitRestriction: return !ServerGame.Cards.Any(c => cardRestriction.Evaluate(c));
                case MustBeFriendlyTurn: return ServerGame.TurnPlayer != Effect.Controller;
                case MustBeEnemyTurn:    return ServerGame.TurnPlayer == Effect.Controller;
                case TargetViolatesRestriction: return !cardRestriction.Evaluate(Target);
                case TargetFitsRestriction:     return cardRestriction.Evaluate(Target);
                default: throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect");
            }
        }

        public override bool Resolve()
        {
            if (ShouldEnd(condition)) return ServerEffect.EndResolution();
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}