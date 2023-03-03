using KompasCore.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects.Subeffects
{
    public class AutoTargetSpaceIdentity : ServerSubeffect
    {
            public INoActivationContextIdentity<Space> subeffectSpaceIdentity;

            public override void Initialize(ServerEffect eff, int subeffIndex)
            {
                base.Initialize(eff, subeffIndex);
                subeffectSpaceIdentity.Initialize(initializationContext: DefaultInitializationContext);
            }

            public override Task<ResolutionInfo> Resolve()
            {
                var space = subeffectSpaceIdentity.Item;
                if (space == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidCardTarget));

                Effect.AddSpace(space);
                return Task.FromResult(ResolutionInfo.Next);
            }
    }
}