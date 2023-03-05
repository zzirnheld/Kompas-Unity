using KompasCore.Effects.Identities.ActivationContextNumberIdentities;

namespace KompasServer.Effects.Subeffects
{
    public class SpendMovement : UpdateCardStats
    {
        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            spacesMoved = new X() { multiplier = xMultiplier, modifier = xModifier, divisor = xDivisor };
            base.Initialize(eff, subeffIndex);
        }
    }
}