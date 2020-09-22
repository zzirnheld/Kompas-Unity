namespace KompasServer.Effects
{
    public class AddPipsSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            Player.Pips += Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}