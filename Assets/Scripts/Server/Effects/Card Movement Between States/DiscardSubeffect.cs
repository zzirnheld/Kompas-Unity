namespace KompasServer.Effects
{
    public class DiscardSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            if (Target != null && Target.Discard(ServerEffect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}