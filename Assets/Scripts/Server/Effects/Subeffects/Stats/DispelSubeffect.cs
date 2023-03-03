using KompasCore.Cards.Movement;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
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