namespace KompasServer.Effects
{
    public class TargetThisSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.AddTarget(ThisCard);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}