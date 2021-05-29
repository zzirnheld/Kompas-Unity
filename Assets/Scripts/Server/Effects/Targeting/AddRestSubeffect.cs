using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AddRestSubeffect : CardTargetSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.rest.AddRange(ServerGame.Cards.Where(cardRestriction.Evaluate));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}