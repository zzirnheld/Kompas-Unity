using KompasCore.Effects;

namespace KompasServer.Effects.Subeffects
{
    public class SetXByTriggeringCardValue : SetX
    {
        public CardValue cardValue;
        public bool secondary = false;

        public override void Initialize(ServerEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);

            cardValue?.Initialize(DefaultInitializationContext);
        }

        public override int BaseCount => cardValue.GetValueOf(secondary ? 
            CurrentContext.secondaryCardInfoBefore :
            CurrentContext.mainCardInfoBefore);
    }
}