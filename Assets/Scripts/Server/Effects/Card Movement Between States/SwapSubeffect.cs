using KompasCore.Cards;

namespace KompasServer.Effects
{
    public class SwapSubeffect : ServerSubeffect
    {
        public int SecondTargetIndex = -2;
        public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);

        public override bool Resolve()
        {
            if (Target != null && SecondTarget != null && Target.Move(SecondTarget.BoardX, SecondTarget.BoardY, false, ServerEffect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}