using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class CauseBefore : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => contextToConsider.cardCauseBefore;
    }
}