using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class ChangeXByTargetValueSubeffect : SetXByTargetValueSubeffect
    {
        public override int BaseCount => Effect.X + base.BaseCount;
    }
}