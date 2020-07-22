namespace KompasServer.Effects
{
    public class TargetThisSubeffect : CardTargetSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.AddTarget(ThisCard);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}