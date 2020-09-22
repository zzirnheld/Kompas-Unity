namespace KompasServer.Effects
{
    public class DrawXSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            var drawn = ServerGame.DrawX(Player.index, Count);
            if (drawn.Count < Count) return ServerEffect.EffectImpossible();
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}