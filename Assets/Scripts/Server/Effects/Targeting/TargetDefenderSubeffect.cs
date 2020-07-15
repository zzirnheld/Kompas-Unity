public class TargetDefenderSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if (ServerEffect.CurrActivationContext.Stackable is Attack attack)
        {
            ServerEffect.AddTarget(attack.defender);
            ServerEffect.ResolveNextSubeffect();
        }
        else ServerEffect.EffectImpossible();
    }
}
