namespace KompasServer.Effects
{
    public class TopdeckSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            if (Target != null && Target.Topdeck(Target.Owner, Effect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}