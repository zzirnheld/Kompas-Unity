using KompasCore.Effects.Identities.Numbers;

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