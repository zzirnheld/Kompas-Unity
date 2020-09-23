using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class TargetAttackerSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.Stackable is Attack attack)
            {
                ServerEffect.AddTarget(attack.attacker);
                return ServerEffect.ResolveNextSubeffect();
            }
            else return ServerEffect.EffectImpossible();
        }
    }
}