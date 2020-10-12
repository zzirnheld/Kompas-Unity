using KompasCore.Cards;
using KompasCore.Effects;

namespace KompasServer.Effects
{
    public class TargetOtherInFightSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (ServerEffect.CurrActivationContext.Stackable is Attack attack)
            {
                GameCard newTarget = null;
                if (attack.attacker == Target) newTarget = attack.defender;
                else if (attack.defender == Target) newTarget = attack.attacker;

                if (newTarget != null)
                {
                    ServerEffect.AddTarget(newTarget);
                    return ServerEffect.ResolveNextSubeffect();
                }
            }
            
            return ServerEffect.EffectImpossible();
        }
    }
}