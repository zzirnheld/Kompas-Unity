using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXByTriggeringCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardValue?.Initialize(eff.Source);
        }

        public override int BaseCount => cardValue.GetValueOf(Context.mainCardInfoBefore);
    }
}