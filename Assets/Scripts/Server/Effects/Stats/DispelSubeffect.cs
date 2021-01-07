using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class DispelSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Target.Dispel(Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}