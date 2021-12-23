using System.Threading.Tasks;

namespace KompasServer.Effects
{
    /// <summary>
    /// Represents an entire keyword part of an effect,
    /// like Mech Pilot (or, hopefully soon, Invoke)
    /// </summary>
    public class KeywordSubeffect : ServerSubeffect
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
        }

        public override Task<ResolutionInfo> Resolve() => Task.FromResult(ResolutionInfo.Next);
    }
}