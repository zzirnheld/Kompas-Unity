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
        public const string PlayerValueFitsNumberRestriction = "Player Value Fits Number Restriction";
        public const string PlayerValueFloutsNumberRestriction = "Player Value Flouts Number Restriction";

        public int constant = 0;
        public CardRestriction cardRestriction;

        public PlayerValue playerValue;
        public NumberRestriction playerValueNumberRestriction;

        public string condition;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction ??= new CardRestriction();
            cardRestriction.Initialize(this);

            playerValueNumberRestriction?.Initialize(eff.Source, this);
        }

        private bool ShouldEnd => condition switch
        {
            XLessThan0                          => ServerEffect.X < 0,
            XLessThanEqual0                     => ServerEffect.X <= 0,
            XGreaterThanConst                   => ServerEffect.X > constant,
            XLessThanConst                      => ServerEffect.X < constant,

            NoneFitRestriction                  => !ServerGame.Cards.Any(c => cardRestriction.IsValidCard(c, Context)),
            AnyFitRestriction                   => ServerGame.Cards.Any(c => cardRestriction.IsValidCard(c, Context)),

            MustBeFriendlyTurn                  => ServerGame.TurnPlayer != Effect.Controller,
            MustBeEnemyTurn                     => ServerGame.TurnPlayer == Effect.Controller,

            TargetViolatesRestriction           => !cardRestriction.IsValidCard(CardTarget, Context),
            TargetFitsRestriction               => cardRestriction.IsValidCard(CardTarget, Context),

            SourceViolatesRestriction           => !cardRestriction.IsValidCard(Source, Context),
            NumTargetsLTEConstant               => Effect.CardTargets.Count() <= constant,
            HandFull                            => PlayerTarget.handCtrl.HandSize >= Controller.HandSizeLimit,
            PlayerValueFitsNumberRestriction    => playerValueNumberRestriction.IsValidNumber(playerValue.GetValueOf(PlayerTarget)),
            PlayerValueFloutsNumberRestriction  => !playerValueNumberRestriction.IsValidNumber(playerValue.GetValueOf(PlayerTarget)),
            _ => throw new System.ArgumentException($"Condition {condition} invalid for conditional end subeffect"),
        };

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldEnd) return Task.FromResult(ResolutionInfo.End(condition));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}