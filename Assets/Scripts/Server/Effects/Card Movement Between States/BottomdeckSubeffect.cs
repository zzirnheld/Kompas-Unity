namespace KompasServer.Effects
{
    public class BottomdeckSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            if (Target != null && Target.Bottomdeck(Target.Owner, Effect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}