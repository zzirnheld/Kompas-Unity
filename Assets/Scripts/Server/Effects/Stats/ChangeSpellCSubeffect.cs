namespace KompasServer.Effects
{
    public class ChangeSpellCSubeffect : ServerSubeffect
    {public override bool Resolve()
        {
            Target.SetC(Count, Effect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}