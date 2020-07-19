namespace KompasServer.Effects
{
    public class ThisSpaceTargetSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.Coords.Add((ThisCard.BoardX, ThisCard.BoardY));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}