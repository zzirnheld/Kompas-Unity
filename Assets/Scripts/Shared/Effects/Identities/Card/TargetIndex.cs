using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class TargetIndex : EffectContextualLeafIdentityBase<GameCardBase>
    {
        public int index = -1;

        protected override GameCardBase AbstractItemFrom(IResolutionContext contextToConsider)
            => EffectHelpers.GetItem(contextToConsider.CardTargets, index);
    }
}