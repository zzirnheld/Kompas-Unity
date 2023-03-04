using KompasServer.Effects.Identities.SubeffectNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class ChangeDuration : UpdateCardStats
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            durationChange ??= new X();
            base.Initialize(eff, subeffIndex);
        }
    }
}