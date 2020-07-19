namespace KompasServer.Effects
{
    public class SetXTargetSSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            ServerEffect.X = Target.S;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}