namespace KompasServer.Effects
{
    public class SetXSubeffect : ServerSubeffect
    {
        public virtual int BaseCount => Effect.X;

        public int TrueCount => (BaseCount * xMultiplier / xDivisor) + xModifier;

        public override bool Resolve()
        {
            Effect.X = TrueCount;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}