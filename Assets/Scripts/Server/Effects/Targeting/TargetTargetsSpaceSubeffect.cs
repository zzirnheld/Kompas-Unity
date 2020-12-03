namespace KompasServer.Effects
{
    public class TargetTargetsSpaceSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.coords.Add(Target.Position);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}