using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class CauseAfter : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }
}