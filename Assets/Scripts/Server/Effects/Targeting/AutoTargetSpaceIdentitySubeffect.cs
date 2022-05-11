using KompasCore.Effects.Identities;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class AutoTargetSpaceIdentitySubeffect : ServerSubeffect
    {
        public INoActivationContextSpaceIdentity spaceIdentity;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceIdentity.Initialize(restrictionContext: DefaultRestrictionContext);
        }

        public override Task<ResolutionInfo> Resolve()
        {
            var space = spaceIdentity.Space;
            if (space == null) return Task.FromResult(ResolutionInfo.Impossible(NoValidSpaceTarget));

            Effect.AddSpace(space);
            return Task.FromResult(ResolutionInfo.Next);
        }
    }
}
