namespace KompasServer.Effects
{
    public class TargetAvatarSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.AddTarget(Player.Avatar);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}