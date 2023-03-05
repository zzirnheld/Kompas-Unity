using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Leaf.Cards
{
    public class ThisCard : ContextlessLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItem => InitializationContext.source;
    }
}