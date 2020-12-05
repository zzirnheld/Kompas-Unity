namespace KompasServer.Effects
{
    public class TargetThisSpaceSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.coords.Add((ThisCard.BoardX, ThisCard.BoardY));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}