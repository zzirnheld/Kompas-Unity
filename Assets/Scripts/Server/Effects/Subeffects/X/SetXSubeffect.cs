using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class SetXSubeffect : ServerSubeffect
    {
        public virtual int BaseCount => Effect.X;

        public int TrueCount => (BaseCount * xMultiplier / xDivisor) + xModifier + (change ? Effect.X : 0);

        public bool change = false;

        public override Task<ResolutionInfo> Resolve()
        {
            Effect.X = TrueCount;
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}