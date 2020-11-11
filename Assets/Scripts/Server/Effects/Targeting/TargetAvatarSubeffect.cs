namespace KompasServer.Effects
{
    public class TargetAvatarSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.AddTarget(Controller.Avatar);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}