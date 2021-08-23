using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXByTriggeringCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;

        public override int BaseCount => cardValue.GetValueOf(Context.CardInfo);
    }
}