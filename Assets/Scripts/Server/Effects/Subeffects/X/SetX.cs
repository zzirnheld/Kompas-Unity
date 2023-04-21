using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects.Subeffects
{
    public class SetX : ServerSubeffect
    {
        public virtual int BaseCount => Effect.X;

        public int TrueCount => (BaseCount * xMultiplier / xDivisor) + xModifier + (change ? Effect.X : 0);

        public bool change = false;

        public override Task<ResolutionInfo> Resolve()
        {
            Effect.X = TrueCount;
            Debug.Log($"Setting X to {Effect.X}");
            return Task.FromResult(ResolutionInfo.Next);
        }

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            if (xModifier != 0) Debug.LogWarning($"X modifier {xModifier}nonstandard");
            if (xMultiplier != 0) Debug.LogWarning($"x mulitplier {xMultiplier},  nonstandard");
            if (xDivisor != 1) Debug.LogWarning($"xDivisor {xDivisor} nonstandard");
        }
    }
}