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
        public const string SourceViolatesRestriction = "Source Violates Restriction";
        public const string NumTargetsLTEConstant = "Number Targets <= Constant";

        public int constant = 0;
        public CardRestriction cardRestriction;

        public string condition;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);
        }

        private bool ShouldEnd(string condition)
        {
            return condition switch
            {
                XLessThan0                  => ServerEffect.X < 0,
                XLessThanEqual0             => ServerEffect.X <= 0,
                XGreaterThanConst           => ServerEffect.X > constant,
                XLessThanConst              => ServerEffect.X < constant,
                NoneFitRestriction          => !ServerGame.Cards.Any(c => cardRestriction.Evaluate(c, Effect.CurrActivationContext)),
                AnyFitRestriction           => ServerGame.Cards.Any(c => cardRestriction.Evaluate(c, Effect.CurrActivationContext)),
                MustBeFriendlyTurn          => ServerGame.TurnPlayer != Effect.Controller,
                MustBeEnemyTurn             => ServerGame.TurnPlayer == Effect.Controller,
                TargetViolatesRestriction   => !cardRestriction.Evaluate(Target, Effect.CurrActivationContext),
                TargetFitsRestriction       => cardRestriction.Evaluate(Target, Effect.CurrActivationContext),
                SourceViolatesRestriction   => !cardRestriction.Evaluate(Source, Effect.CurrActivationContext),
                NumTargetsLTEConstant       => Effect.Targets.Count() <= constant,
                _ => throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect"),
            };
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldEnd(condition)) return Task.FromResult(ResolutionInfo.End(condition));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}