using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class EndTurnSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerGame.SwitchTurn();
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}