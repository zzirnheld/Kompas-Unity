namespace KompasServer.Effects
{
    public class SetXByGamestateSubeffect : XByGamestateSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.X = Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}