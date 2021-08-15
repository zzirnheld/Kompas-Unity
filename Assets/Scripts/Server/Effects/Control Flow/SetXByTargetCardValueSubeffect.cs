using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXByTargetCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;

        public override int BaseCount => cardValue.GetValueOf(Target);
    }
}