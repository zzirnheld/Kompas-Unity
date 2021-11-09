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
        public const string HandFull = "Hand Full";

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
                NoneFitRestriction          => !ServerGame.Cards.Any(c => cardRestriction.Evaluate(c, Context)),
                AnyFitRestriction           => ServerGame.Cards.Any(c => cardRestriction.Evaluate(c, Context)),
                MustBeFriendlyTurn          => ServerGame.TurnPlayer != Effect.Controller,
                MustBeEnemyTurn             => ServerGame.TurnPlayer == Effect.Controller,
                TargetViolatesRestriction   => !cardRestriction.Evaluate(Target, Context),
                TargetFitsRestriction       => cardRestriction.Evaluate(Target, Context),
                SourceViolatesRestriction   => !cardRestriction.Evaluate(Source, Context),
                NumTargetsLTEConstant       => Effect.Targets.Count() <= constant,
                HandFull                    => Player.handCtrl.HandSize >= HandSizeStackable.MaxHandSize,
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