using KompasCore.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AutoTargetSpaceIdentitySubeffect : ServerSubeffect
    {
        public INoActivationContextIdentity<Space> spaceIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceIdentity.Initialize(initializationContext: DefaultInitializationContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var space = spaceIdentity.Item;
            if (space == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

            Effect.AddSpace(space);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
