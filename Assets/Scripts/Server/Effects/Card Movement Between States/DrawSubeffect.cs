namespace KompasServer.Effects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public int Player = -1;
        public int WhoDraws { get { return Player == -1 ? ServerEffect.ServerController.index : Player; } }

        public override bool Resolve()
        {
            var drawn = ServerGame.Draw(WhoDraws);
            if (drawn == null) return ServerEffect.EffectImpossible();
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}