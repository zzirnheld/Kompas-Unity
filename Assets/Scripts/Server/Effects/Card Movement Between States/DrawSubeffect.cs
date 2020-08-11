namespace KompasServer.Effects
{
    public class DrawSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            var drawn = ServerGame.Draw(Player.index);
            if (drawn == null) return ServerEffect.EffectImpossible();
            else return ServerEffect.ResolveNextSubeffect();
        }
    }
}