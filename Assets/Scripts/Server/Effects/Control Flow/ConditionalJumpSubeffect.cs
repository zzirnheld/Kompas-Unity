using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class ConditionalJumpSubeffect : ServerSubeffect
    {
        public const string TargetFitsRestriction = "Target Fits Restriction";
        public const string XGreaterEqualConstant = "X >= Constant";

        public string condition;
        public int jumpIndex;

        public CardRestriction cardRestriction = new CardRestriction();
        public int constant;

        private bool ShouldJump
        {
            get
            {
                switch (condition)
                {
                    case TargetFitsRestriction: return cardRestriction.Evaluate(Target);
                    case XGreaterEqualConstant: return Effect.X >= constant;
                    default: throw new System.ArgumentException($"Invalid conditional jump condition {condition}");
                }
            }
        }

        public override Task<ResolutionInfo> Resolve()
        {
            if (ShouldJump) return Task.FromResult(ResolutionInfo.Index(jumpIndex));
            else return Task.FromResult(ResolutionInfo.Next);
        }
    }
}