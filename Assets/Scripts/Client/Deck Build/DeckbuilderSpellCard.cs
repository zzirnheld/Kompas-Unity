using KompasCore.Cards;

namespace KompasDeckbuilder
{
    public class DeckbuilderSpellCard : DeckbuilderCard
    {
        public override string BlurbString => $"C{C}{(Fast ? " Fast" : "")} {SubtypeText}";
    }
}