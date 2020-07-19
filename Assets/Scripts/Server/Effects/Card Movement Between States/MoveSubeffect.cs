namespace KompasServer.Effects
{
    public class MoveSubeffect : CardChangeStateSubeffect
    {
        public override bool Resolve()
        {
            var (x, y) = Space;
            if (Target != null && Target.Move(x, y, false, Effect))
                return ServerEffect.ResolveNextSubeffect();
            else return ServerEffect.EffectImpossible();
        }
    }
}