using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class MainCardBefore : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext context)
            => context.mainCardInfoBefore;
    }
}