using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class TargetTargetsSpaceSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            if (Target.Location != CardLocation.Field) 
                return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

            Effect.AddSpace(Target.Position.Copy);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}