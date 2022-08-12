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
        public HashSet<int> CardIDs { get; }
        public Effect LinkingEffect { get; }

        public CardLink(HashSet<int> cardIDs, Effect linkingEffect)
        {
            CardIDs = cardIDs;
            LinkingEffect = linkingEffect;
        }

        public bool Matches(IEnumerable<int> cardIDs, Effect linkingEffect)
        {
            return LinkingEffect == linkingEffect && CardIDs.SetEquals(cardIDs);
        }
    }
}
