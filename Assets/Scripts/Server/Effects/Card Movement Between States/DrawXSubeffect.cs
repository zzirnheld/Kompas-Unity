namespace KompasServer.Effects
{
    public class DrawXSubeffect : ServerSubeffect
    {
        public int Player = -1;
        public int WhoDraws { get { return Player == -1 ? ServerEffect.ServerController.index : Player; } }
        public int XMultiplier = 1;
        public int XDivisor = 1;
        public int modifier = 0;
        public int Count => (ServerEffect.X * XMultiplier / XDivisor) + modifier;

        public override bool Resolve()
        {
            var drawn = ServerGame.DrawX(WhoDraws, Count);
            if (drawn.Count < Count) return ServerEffect.EffectImpossible();
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}