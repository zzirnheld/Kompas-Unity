using KompasCore.Effects;
using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class ConditionalJump : ServerSubeffect
    {
        public const string NoCardFitsRestriction = "No Card Fits Restriction";
        public const string CardFitsRestriction = "Card Fits Restriction";

        public const string TargetFitsRestriction = "Target Fits Restriction";
        public const string TargetViolatesRestriction = "Target Violates Restriction";

        public const string MainTriggeringCardFitRestrictionBefore = "Main Triggering Card Fit Restriction Before";
        public const string MainTriggeringCardFloutedRestrictionBefore = "Main Triggering Card Flouted Restriction Before";

        public const string XGreaterEqualConstant = "X >= Constant";
        public const string XFitsRestriction = "X Fits Restriction";

        public string condition;

        public CardRestriction cardRestriction = new CardRestriction();
        public NumberRestriction xRestriction = new NumberRestriction();
        public int constant;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            cardRestriction.Initialize(DefaultInitializationContext);
            xRestriction.Initialize(DefaultInitializationContext);
        }

        private bool ShouldJump
        {
            get
            {
                return condition switch
                {
                    CardFitsRestriction => Game.Cards.Any(c => cardRestriction.IsValidCard(c, CurrentContext)),
                    NoCardFitsRestriction => !Game.Cards.Any(c => cardRestriction.IsValidCard(c, CurrentContext)),

                    TargetFitsRestriction => cardRestriction.IsValidCard(CardTarget, CurrentContext),
                    TargetViolatesRestriction => !cardRestriction.IsValidCard(CardTarget, CurrentContext),

                    MainTriggeringCardFitRestrictionBefore => cardRestriction.IsValidCard(CurrentContext.mainCardInfoBefore, CurrentContext),
                    MainTriggeringCardFloutedRestrictionBefore => !cardRestriction.IsValidCard(CurrentContext.mainCardInfoBefore, CurrentContext),

                    XGreaterEqualConstant => Effect.X >= constant,
                    XFitsRestriction => xRestriction.IsValidNumber(Effect.X),
                    _ => throw new System.ArgumentException($"Invalid conditional jump condition {condition}"),
                };
            }
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldJump) return Task.FromResult(ResolutionInfo.Index(JumpIndex));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}