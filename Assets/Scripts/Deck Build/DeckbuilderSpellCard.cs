namespace KompasDeckbuilder
{
    public class DeckbuilderSpellCard : DeckbuilderCard
    {
        public override string BlurbString => StatsString + QualifiedSubtypeText;
    }
}