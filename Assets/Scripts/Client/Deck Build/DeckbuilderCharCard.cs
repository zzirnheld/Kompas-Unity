using KompasCore.Cards;

namespace KompasDeckbuilder
{
    public class DeckbuilderCharCard : DeckbuilderCard 
    {
        public override string BlurbString => $"N{N} / E{E} / S{S} / W{W} {SubtypeText}";
    }
}