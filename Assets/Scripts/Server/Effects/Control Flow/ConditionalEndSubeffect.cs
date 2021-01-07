using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ConditionalEndSubeffect : ServerSubeffect
    {
        public const string XLessThan0 = "X<0";
        public const string XLessThanEqual0 = "X<=0";
        public const string XGreaterThanConst = "X>C";
        public const string XLessThanConst = "X<C";
        public const string NoneFitRestriction = "None Fit Restriction";
        public const string AnyFitRestriction = "Any Fit Restriction";
        public const string MustBeFriendlyTurn = "Must be Friendly Turn";
        public const string MustBeEnemyTurn = "Must be Enemy Turn";
        public const string TargetViolatesRestriction = "Target Violates Restriction";
        public const string TargetFitsRestriction = "Target Fits Restriction";
        public const string NumTargetsLTEConstant = "Number Targets <= Constant";

        public int constant = 0;
        public CardRestriction cardRestriction;

        public string condition;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction = cardRestriction ?? new CardRestriction();
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
                case AnyFitRestriction:  return ServerGame.Cards.Any(c => cardRestriction.Evaluate(c));
                case MustBeFriendlyTurn: return ServerGame.TurnPlayer != Effect.Controller;
                case MustBeEnemyTurn:    return ServerGame.TurnPlayer == Effect.Controller;
                case TargetViolatesRestriction: return !cardRestriction.Evaluate(Target);
                case TargetFitsRestriction:     return cardRestriction.Evaluate(Target);
                case NumTargetsLTEConstant:     return Effect.Targets.Count() <= constant;
                default: throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect");
            }
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldEnd(condition)) return Task.FromResult(ResolutionInfo.End(condition));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}