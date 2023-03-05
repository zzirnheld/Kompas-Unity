using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class CauseAfter : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.CauseCardInfoAfter;
    }
}