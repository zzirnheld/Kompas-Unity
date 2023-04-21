using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class TargetIndex : EffectContextualCardIdentityBase
    {
        public int index = -1;

        protected override GameCardBase AbstractItemFrom(IResolutionContext contextToConsider)
        {
            return InitializationContext.effect?.identityOverrides.TargetCardOverride
                ?? EffectHelpers.GetItem(contextToConsider.CardTargets, index);
        } 
    }
}