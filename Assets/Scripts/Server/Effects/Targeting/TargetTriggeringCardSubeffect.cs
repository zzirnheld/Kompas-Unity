namespace KompasServer.Effects
{
    public class TargetTriggeringCardSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.CardInfo == null)
                return ServerEffect.EffectImpossible();

            ServerEffect.AddTarget(ServerEffect.CurrActivationContext.CardInfo.Card);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}