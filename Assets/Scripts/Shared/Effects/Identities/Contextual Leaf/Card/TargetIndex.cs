using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class TargetIndex : ContextualLeafIdentityBase<GameCardBase>
    {
        public int index = -1;

        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => EffectHelpers.GetItem(contextToConsider.CardTargets, index);
    }
}