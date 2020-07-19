namespace KompasServer.Effects
{
    public class TargetThisSpaceSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.Coords.Add((ThisCard.BoardX, ThisCard.BoardY));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}