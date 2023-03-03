namespace KompasServer.Effects.Subeffects
{
    public class SetXByTargetCostSubeffect : SetXSubeffect
    {
        public override int BaseCount => CardTarget?.Cost ?? 0;
    }
}