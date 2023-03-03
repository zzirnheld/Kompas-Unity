using KompasCore.Effects.Identities.ActivationContextCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetDefender : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new Defender();
            base.Initialize(eff, subeffIndex);
        }
    }
}