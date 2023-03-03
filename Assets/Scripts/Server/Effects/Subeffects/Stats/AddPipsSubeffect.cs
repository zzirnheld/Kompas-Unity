using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AddPipsSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            PlayerTarget.Pips += Count;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}