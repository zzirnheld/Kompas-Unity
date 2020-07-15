public class PlaySubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        var (x, y) = Space;
        if (Target.Play(x, y, EffectController, Effect)) ServerEffect.ResolveNextSubeffect();
        else ServerEffect.EffectImpossible();
    }
}
