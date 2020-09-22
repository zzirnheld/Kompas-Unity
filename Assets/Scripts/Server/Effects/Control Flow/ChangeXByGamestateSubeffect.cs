namespace KompasServer.Effects
{
    public class ChangeXByGamestateSubeffect : SetXByGamestateSubeffect
    {
        public override int BaseCount => Effect.X + base.BaseCount;
    }
}