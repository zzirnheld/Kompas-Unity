namespace KompasServer.Effects
{
    public class AddPipsSubeffect : ServerSubeffect
    {
        public int xMultiplier = 0;
        public int xDivisor = 1;
        public int modifier = 0;
        private int Count => (xMultiplier * ServerEffect.X / xDivisor) + modifier;

        public override bool Resolve()
        {
            Player.Pips += Count;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}