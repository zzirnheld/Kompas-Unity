namespace KompasServer.Effects
{
    public class PayPipsSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            int toPay = Count;
            if (Player.Pips < toPay) return ServerEffect.EffectImpossible();

            Player.Pips -= toPay;
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}