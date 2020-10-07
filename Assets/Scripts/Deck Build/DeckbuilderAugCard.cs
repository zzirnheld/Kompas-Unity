using KompasCore.Cards;

namespace KompasDeckbuilder
{
    public class DeckbuilderAugCard : DeckbuilderCard
    {
        public override string BlurbString => $"A{A}{(Fast ? " Fast" : "")} {SubtypeText}";
    }
}