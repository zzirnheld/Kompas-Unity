using KompasCore.Effects.Identities.ActivationContextPlayerIdentities;
using KompasCore.Effects.Identities.Cards;

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