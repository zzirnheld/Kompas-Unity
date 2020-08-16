namespace KompasServer.Effects
{
    public class TargetTargetsSpaceSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Effect.Coords.Add(Target.Position);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}