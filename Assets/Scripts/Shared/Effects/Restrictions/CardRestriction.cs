using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects
{
    public class CardRestriction : RestrictionBase<GameCardBase>
    {
        public string blurb = "";

        public GameCard Source => InitializationContext.source;

        public override string ToString() => $"Card Restriction of {Source?.CardName}." +
            $"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";

    }
}