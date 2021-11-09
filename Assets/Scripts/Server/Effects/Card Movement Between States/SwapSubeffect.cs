using KompasCore.Cards;
using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SwapSubeffect : ServerSubeffect
    {
        public int SecondTargetIndex = -2;
        public GameCard SecondTarget => Effect.GetTarget(SecondTargetIndex);
        public override bool IsImpossible() => Target == null || SecondTarget == null;

        public override Task<ResolutionInfo> Resolve()
        {
            if (Target == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && Target.Location != CardLocation.Field)
                throw new InvalidLocationException(Target.Location, Target, MovedCardOffBoard);

            if (SecondTarget == null)
                throw new NullCardException(TargetWasNull);
            else if (forbidNotBoard && SecondTarget.Location != CardLocation.Field)
                throw new InvalidLocationException(SecondTarget.Location, SecondTarget, MovedCardOffBoard);

            Target.Move(SecondTarget.Position, false, ServerEffect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}