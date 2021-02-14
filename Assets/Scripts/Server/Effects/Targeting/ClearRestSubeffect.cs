using System.Threading.Tasks;

namespace KompasServer.Effects
{
    [System.Serializable]
    public class ClearRestSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            Effect.rest.Clear();
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}