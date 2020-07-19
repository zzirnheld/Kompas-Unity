namespace KompasServer.Effects
{
    public class SetXByTargetCostSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target == null) return ServerEffect.EffectImpossible();

            ServerEffect.X = Target.Cost;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}