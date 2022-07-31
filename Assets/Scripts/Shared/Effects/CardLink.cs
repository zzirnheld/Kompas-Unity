using KompasCore.Cards;
using System.Collections.Generic;

namespace KompasCore.Effects
{
    /// <summary>
    /// NOTE: Never edit a link after creating it. Delete it and create a new one.
    /// Otherwise, the client won't know the difference between two links of the same cards, from the same effects,
    /// which is a scenario I want to allow. (Two activations of the same card's effect linking the same cards a second time over)
    /// </summary>
    public class CardLink
    {
        public HashSet<GameCard> Cards { get; }
        public Effect LinkingEffect { get; }

        public CardLink(HashSet<GameCard> cards, Effect linkingEffect)
        {
            Cards = cards;
            LinkingEffect = linkingEffect;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is CardLink cardLink)) return false;

            return LinkingEffect == cardLink.LinkingEffect && Cards.SetEquals(cardLink.Cards);
        }
    }
}
