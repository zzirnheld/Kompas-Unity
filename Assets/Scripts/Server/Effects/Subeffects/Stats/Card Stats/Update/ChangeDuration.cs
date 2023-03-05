using KompasCore.Effects.Identities.ActivationContextNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class ChangeDuration : UpdateCardStats
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            duration ??= new X();
            base.Initialize(eff, subeffIndex);
        }
    }
}