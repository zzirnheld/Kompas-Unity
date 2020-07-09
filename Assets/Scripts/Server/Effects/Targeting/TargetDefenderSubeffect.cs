public class TargetDefenderSubeffect : ServerSubeffect
{
    public override void Resolve()
    {
        if (ServerEffect.ServerTrigger.LastTriggerInfo.stack is Attack attack)
        {
            ServerEffect.AddTarget(attack.defender);
            ServerEffect.ResolveNextSubeffect();
        }
        else ServerEffect.EffectImpossible();
    }
}
