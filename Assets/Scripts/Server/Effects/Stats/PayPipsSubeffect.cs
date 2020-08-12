namespace KompasServer.Effects
{
    public class PayPipsSubeffect : ServerSubeffect
    {
        public int xMultiplier = 1;
        public int xDivisor = 1;
        public int modifier = 0;

        public override bool Resolve()
        {
            int toPay = ServerEffect.X * xMultiplier / xDivisor + modifier;
            if (Player.Pips < toPay) return ServerEffect.EffectImpossible();

            Player.Pips -= toPay;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}