using KompasCore.Exceptions;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AttachSubeffect : ServerSubeffect
    {
        //the index for the card to be attached to.
        //default is two targets ago
        public int attachmentTarget = -2;

        public override bool IsImpossible() => Target == null || Effect.GetTarget(attachmentTarget) == null;

        public override Task<ResolutionInfo> Resolve()
        {
            var toAttach = Target;
            var attachTo = Effect.GetTarget(attachmentTarget);

            //if everything goes to plan, resolve the next subeffect
            if (toAttach == null) throw new NullCardException(TargetWasNull);
            else if (attachTo == null) throw new NullCardException(TargetWasNull);

            attachTo.AddAugment(toAttach, stackSrc: Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}