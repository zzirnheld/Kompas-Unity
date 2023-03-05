using KompasCore.Effects.Identities.ActivationContextPlayerIdentities;
using KompasCore.Effects.Identities.GamestateCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetAvatar : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new Avatar() { player = new TargetIndex() };
            base.Initialize(eff, subeffIndex);
        }
    }
}