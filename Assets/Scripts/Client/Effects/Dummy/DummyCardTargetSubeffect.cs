using KompasCore.Effects;

namespace KompasClient.Effects
{
    public class DummyCardTargetSubeffect : DummySubeffect
    {
        public CardRestriction cardRestriction;

        public override void Initialize(ClientEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
        }
    }
}
