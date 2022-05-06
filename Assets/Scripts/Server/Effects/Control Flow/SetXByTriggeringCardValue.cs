using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class SetXByTriggeringCardValueSubeffect : SetXSubeffect
    {
        public CardValue cardValue;
        public bool secondary = false;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardValue?.Initialize(eff.Source);
        }

        public override int BaseCount => cardValue.GetValueOf(secondary ? 
            Context.secondaryCardInfoBefore :
            Context.mainCardInfoBefore);
    }
}