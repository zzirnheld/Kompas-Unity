using KompasCore.Effects;
using System.Threading.Tasks;

namespace KompasServer.Effects
{
    public class SetXByTriggeringCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;

        public override int BaseCount => cardValue.GetValueOf(Effect.CurrActivationContext.CardInfo);
    }
}