using KompasCore.Exceptions;
using KompasServer.Effects.Identities.SubeffectNumberIdentities;
using System.Threading.Tasks;

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