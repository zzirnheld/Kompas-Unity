namespace KompasServer.Effects
{
    public class TargetTriggeringCoordsSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.Space == null)
                return ServerEffect.EffectImpossible();

            ServerEffect.Coords.Add(ServerEffect.CurrActivationContext.Space.Value);
            return ServerEffect.ResolveNextSubeffect();
        }
    }
}