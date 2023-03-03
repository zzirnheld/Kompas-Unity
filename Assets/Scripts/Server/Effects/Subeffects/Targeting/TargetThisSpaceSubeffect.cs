using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class TargetThisSpaceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (ThisCard.Location != CardLocation.Board)
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            ServerEffect.AddSpace(ThisCard.Position);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}