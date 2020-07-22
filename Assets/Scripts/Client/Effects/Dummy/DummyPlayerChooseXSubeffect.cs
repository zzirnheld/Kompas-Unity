using KompasCore.Effects;

namespace KompasClient.Effects
{
    public class DummyPlayerChooseXSubeffect : DummySubeffect
    {
        public XRestriction XRest;

        public override void Initialize(ClientEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            XRest.Initialize(this);
        }
    }
}