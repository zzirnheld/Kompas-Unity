public class PlaySubeffect : CardChangeStateSubeffect
{
    public override void Resolve()
    {
        var (x, y) = Space;
        Target.Play(x, y, EffectController, Effect);
        ServerEffect.ResolveNextSubeffect();
    }
}
