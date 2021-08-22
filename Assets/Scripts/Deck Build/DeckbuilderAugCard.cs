namespace KompasDeckbuilder
{
    public class DeckbuilderAugCard : DeckbuilderCard
    {
        public override string BlurbString => StatsString + QualifiedSubtypeText;
    }
}