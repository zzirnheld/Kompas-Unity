using System.Linq;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class AddRestSubeffect : CardTarget
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.rest.AddRange(ServerGame.Cards.Where(c => cardRestriction.IsValid(c, ResolutionContext)));
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}