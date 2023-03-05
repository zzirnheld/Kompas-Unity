using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Cards
{
    public class ThisCard : ContextlessLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItem => InitializationContext.source;
    }
}