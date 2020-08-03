using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ConditionalJumpSubeffect : ServerSubeffect
    {
        public const string TargetFitsRestriction = "Target Fits Restriction";

        public string condition;
        public int jumpIndex;

        public CardRestriction cardRestriction = new CardRestriction();

        private bool ShouldJump
        {
            get
            {
                switch (condition)
                {
                    case TargetFitsRestriction: return cardRestriction.Evaluate(Target);
                    default: throw new System.ArgumentException($"Invalid conditional jump condition {condition}");
                }
            }
        }

        public override bool Resolve()
        {
            if (ShouldJump) return ServerEffect.ResolveSubeffect(jumpIndex);
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}