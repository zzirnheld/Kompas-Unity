using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class TargetDefenderSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.Stackable is Attack attack)
            {
                ServerEffect.AddTarget(attack.defender);
                return ServerEffect.ResolveNextSubeffect();
            }
            else return ServerEffect.EffectImpossible();
        }
    }
}