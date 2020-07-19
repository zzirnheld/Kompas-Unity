namespace KompasServer.Effects
{
    public class NegateSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Target.SetNegated(true, ServerEffect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}