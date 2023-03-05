using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Leaf.Card
{
    public class ThisCard : ContextlessLeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItem => InitializationContext.source;
    }
}