namespace KompasServer.Effects
{
    public class JumpSubeffect : ServerSubeffect
    {
        public int indexToJumpTo;

        public override bool Resolve()
        {
            //this will always jump to the given subeffect index
            return ServerEffect.ResolveSubeffect(indexToJumpTo);
        }
    }
}