using KompasCore.Cards;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SwapSubeffect : ServerSubeffect
    {
        public int SecondTargetIndex = -2;
        public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null || SecondTarget == null) return Task.FromResult(ResolutionInfo.Impossible(TargetWasNull));
            else if (Target.Move(SecondTarget.BoardX, SecondTarget.BoardY, false, ServerEffect))
                return Task.FromResult(ResolutionInfo.Next);
            else return Task.FromResult(ResolutionInfo.Impossible(MovementFailed));
        }
    }
}