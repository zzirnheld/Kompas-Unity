namespace KompasServer.Effects.Subeffect
{
    public class SetXByTargetCostSubeffect : SetXSubeffect
    {
        public override int BaseCount => CardTarget?.Cost ?? 0;
    }
}