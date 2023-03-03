using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class EndTurnSubeffect : ServerSubeffect
    {
        public override async Task<ResolutionInfo> Resolve()
        {
            await ServerGame.SwitchTurn();
            return ResolutionInfo.Next;
        }
    }
}