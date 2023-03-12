namespace KompasServer.Effects.Subeffects
{
    public class SetXByTargetCost: SetX
    {
        public override int BaseCount => CardTarget?.Cost ?? 0;
    }
}