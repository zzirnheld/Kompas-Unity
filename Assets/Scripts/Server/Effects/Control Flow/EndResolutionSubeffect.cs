namespace KompasServer.Effects
{
    public class EndResolutionSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            return ServerEffect.ResolveSubeffect(ServerEffect.Subeffects.Length);
        }
    }
}