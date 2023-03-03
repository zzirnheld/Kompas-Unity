using KompasCore.Cards.Movement;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffect
{
    public class DispelSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            CardTarget.Dispel(Effect);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}