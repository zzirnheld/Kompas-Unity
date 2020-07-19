namespace KompasServer.Effects
{
    public class DispelSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Target.Dispel(Effect);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}