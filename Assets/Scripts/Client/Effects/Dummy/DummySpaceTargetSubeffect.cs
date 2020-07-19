using KompasCore.Effects;

namespace KompasClient.Effects
{
    public class DummySpaceTargetSubeffect : DummySubeffect
    {
        public SpaceRestriction spaceRestriction;

        public override void Initialize(ClientEffect eff, int subeffIndex)
        {
            base.Initialize(eff, subeffIndex);
            spaceRestriction.Initialize(this);
        }
    }
}
