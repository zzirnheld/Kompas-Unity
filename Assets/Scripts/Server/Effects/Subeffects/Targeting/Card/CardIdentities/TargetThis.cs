using KompasCore.Effects.Identities.GamestateCardIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class TargetThis : AutoTargetCardIdentity
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            subeffectCardIdentity = new ThisCard();
            base.Initialize(eff, subeffIndex);
        }
    }
}