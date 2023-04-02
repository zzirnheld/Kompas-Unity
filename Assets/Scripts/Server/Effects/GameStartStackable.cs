using KompasCore.Cards;
using KompasCore.Effects;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class GameStartStackable : IStackable, IServerStackable
    {
        public Player Controller => null;

        public GameCard Source => null;

        public ServerPlayer ServerController => null;

        public GameCard GetCause(GameCardBase withRespectTo) => Source;

        public Task StartResolution(ResolutionContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}