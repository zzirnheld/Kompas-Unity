namespace KompasServer.Effects
{
    public class ChangeSpellCSubeffect : ServerSubeffect
    {public override bool Resolve()
        {
            Target.SetC(Target.C + Count, Effect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}