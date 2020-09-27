namespace KompasServer.Effects
{
    public class DeleteTargetSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (RemoveTarget()) return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}