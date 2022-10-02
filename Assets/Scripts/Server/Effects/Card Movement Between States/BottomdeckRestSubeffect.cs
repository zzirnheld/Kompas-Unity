using KompasCore.GameCore;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class BottomdeckRestSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            DeckController.BottomdeckMany(Effect.rest);

            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}