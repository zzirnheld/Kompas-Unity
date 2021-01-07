using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AttachSubeffect : ServerSubeffect
    {
        //the index for the card to be attached to.
        //default is two targets ago
        public int attachmentTarget = -2;

        public override async Task<ResolutionInfo> Resolve()
        {
            var toAttach = Target;
            var attachTo = Effect.GetTarget(attachmentTarget);

            //if everything goes to plan, resolve the next subeffect
            if (toAttach == null || attachTo == null) return ResolutionInfo.Impossible(TargetWasNull);
            else if (attachTo.AddAugment(toAttach, Effect)) return ResolutionInfo.Next;
            else return ResolutionInfo.Impossible(AttachFailed);
        }
    }
}