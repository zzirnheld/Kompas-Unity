using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AddPipsSubeffect : ServerSubeffect
    {
        public override async Task<ResolutionInfo> Resolve()
        {
            Player.Pips += Count;
            return ResolutionInfo.Next;
        }
    }
}