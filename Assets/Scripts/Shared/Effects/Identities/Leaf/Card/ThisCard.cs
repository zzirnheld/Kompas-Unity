using KompasCore.Cards;

namespace KompasCore.Effects.Identities.Leaf.Card
{
    public class ThisCard : LeafIdentityBase<GameCardBase>
    {
        protected override GameCardBase AbstractItem => InitializationContext.source;
    }
}