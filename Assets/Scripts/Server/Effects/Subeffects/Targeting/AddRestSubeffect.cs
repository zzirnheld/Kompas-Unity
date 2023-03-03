using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class AddRestSubeffect : CardTargetSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.rest.AddRange(ServerGame.Cards.Where(c => cardRestriction.IsValidCard(c, CurrentContext)));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}