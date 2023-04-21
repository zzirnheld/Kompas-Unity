using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class ThisCard : ContextlessLeafCardIdentityBase
    {
        protected override GameCardBase AbstractItem => InitializationContext.source;
    }
}