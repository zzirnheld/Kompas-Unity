namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.Space == null)
                return ServerEffect.EffectImpossible();

            ServerEffect.coords.Add(ServerEffect.CurrActivationContext.Space.Value);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}