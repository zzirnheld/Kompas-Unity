using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    /// <summary>
    /// Removes any effect currently set to trigger if an effect is declared impossible.
    /// </summary>
    public class ClearOnImpossibleSubeffect : ServerSubeffect
    {
        public override Task<ResolutionInfo> Resolve()
        {
            ServerEffect.OnImpossible = null;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}