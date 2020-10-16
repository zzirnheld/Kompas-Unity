using KompasCore.Effects;

namespace KompasClient.Effects
{
    public class DummyListTargetSubeffect : DummySubeffect
    {
        /// <summary>
        /// Restriction that each card must fulfill
        /// </summary>
        public CardRestriction cardRestriction = new CardRestriction();

        /// <summary>
        /// Restriction that the list collectively must fulfill
        /// </summary>
        public ListRestriction listRestriction = ListRestriction.Default;

        public override void Initialize(ClientEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            cardRestriction.Initialize(this);
            listRestriction.Initialize(this);
        }
    }
}