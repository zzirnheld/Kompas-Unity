namespace KompasServer.Effects
{
    public class RehandSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            if (Target != null && Target.Rehand(Target.Owner, Effect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}