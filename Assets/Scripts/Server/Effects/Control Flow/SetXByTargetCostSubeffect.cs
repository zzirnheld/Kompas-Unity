namespace KompasServer.Effects
{
    public class SetXByTargetCostSubeffect : SetXSubeffect
    {
        public override int BaseCount => Target?.Cost ?? 0;
    }
}