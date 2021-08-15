namespace KompasDeckbuilder
{
    public class DeckbuilderCharCard : DeckbuilderCard 
    {
        public override string BlurbString => StatsString + QualifiedSubtypeText;
    }
}