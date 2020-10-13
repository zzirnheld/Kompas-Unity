namespace KompasServer.Effects
{
    public class TargetTargetsAugmentedCardSubeffect : ServerSubeffect
    {
        public override bool Resolve()
        {
            if (Target.AugmentedCard == null) return ServerEffect.EffectImpossible();
            else
            {
                ServerEffect.AddTarget(Target.AugmentedCard);
                return ServerEffect.ResolveNextSubeffect();
            }
        }
    }
}