using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class MainCardAfter : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.MainCardInfoAfter;
    }
}