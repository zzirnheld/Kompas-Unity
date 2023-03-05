using KompasCore.Cards;

namespace KompasCore.Effects.Identities.ActivationContextCardIdentities
{
    public class Defender : ContextualLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItemFrom(ActivationContext contextToConsider)
            => GetAttack(contextToConsider).defender;
    }
}