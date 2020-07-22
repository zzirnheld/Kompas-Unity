namespace KompasServer.Effects
{
    public class ChangeXByGamestateSubeffect : XByGamestateSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.X += Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}