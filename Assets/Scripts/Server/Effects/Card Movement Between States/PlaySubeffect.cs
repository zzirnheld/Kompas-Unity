public class PlaySubeffect : CardChangeStateSubeffect
{
    public override bool Resolve()
    {
        var (x, y) = Space;
        if (Target != null && Target.Play(x, y, EffectController, Effect))
            return ServerEffect.ResolveNextSubeffect();
        else return ServerEffect.EffectImpossible();
    }
}
