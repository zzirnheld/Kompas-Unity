namespace KompasServer.Effects
{
    public class SetXByTargetValueSubeffect : ChangeXByTargetValueSubeffect
    {
        public override bool Resolve()
        {
            Effect.X = Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}