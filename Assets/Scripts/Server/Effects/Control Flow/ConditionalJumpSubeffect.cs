using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ConditionalJumpSubeffect : ServerSubeffect
    {
        public const string NoCardFitsRestriction = "No Card Fits Restriction";
        public const string CardFitsRestriction = "Card Fits Restriction";
        public const string TargetFitsRestriction = "Target Fits Restriction";
        public const string TargetViolatesRestriction = "Target Violates Restriction";
        public const string XGreaterEqualConstant = "X >= Constant";
        public const string XFitsRestriction = "X Fits Restriction";

        public string condition;
        public int jumpIndex;

        public CardRestriction cardRestriction = new CardRestriction();
        public XRestriction xRestriction = new XRestriction();
        public int constant;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
            xRestriction.Initialize(Source);
        }

        private bool ShouldJump
        {
            get
            {
                return condition switch
                {
                    CardFitsRestriction         => Game.Cards.Any(c => cardRestriction.Evaluate(c, Context)),
                    NoCardFitsRestriction       => !Game.Cards.Any(c => cardRestriction.Evaluate(c, Context)),
                    TargetFitsRestriction       => cardRestriction.Evaluate(CardTarget, Context),
                    TargetViolatesRestriction   => !cardRestriction.Evaluate(CardTarget, Context),
                    XGreaterEqualConstant       => Effect.X >= constant,
                    XFitsRestriction            => xRestriction.Evaluate(Effect.X),
                    _ => throw new System.ArgumentException($"Invalid conditional jump condition {condition}"),
                };
            }
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldJump) return Task.FromResult(ResolutionInfo.Index(jumpIndex));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}