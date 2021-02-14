using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SpendMovementSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            var spaces = Count;
            if (Target.SpacesCanMove >= spaces)
            {
                Target.SetSpacesMoved(Target.SpacesMoved + spaces);
                return Task.FromResult(ResolutionInfo.Next);
            }
            else return Task.FromResult(ResolutionInfo.Impossible(CantAffordStats));
        }
    }
}