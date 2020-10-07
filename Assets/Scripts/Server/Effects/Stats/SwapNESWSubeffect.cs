namespace KompasServer.Effects
{
    public class SwapNESWSubeffect : ServerSubeffect
    {
        public int[] targetIndices;

        public bool swapN = false;
        public bool swapE = false;
        public bool swapS = false;
        public bool swapW = false;

        public override bool Resolve()
        {
            var target1 = Effect.GetTarget(targetIndices[0]);
            var target2 = Effect.GetTarget(targetIndices[1]);
            if (target1 == null || target2 == null) return ServerEffect.EffectImpossible();

            target1.SwapCharStats(target2, swapN, swapE, swapS, swapW);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}