using KompasCore.Effects.Identities.GamestateCardIdentities;
using KompasServer.Effects.Identities.SubeffectPlayerIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAvatar : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new Avatar() { player = new FromActivationContext() { playerFromContext = new Target() } };
            base.Initialize(eff, subeffIndex);
        }
    }
}