namespace KompasServer.Effects
{
    public class ThisSpaceTargetSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.coords.Add((ThisCard.BoardX, ThisCard.BoardY));
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}