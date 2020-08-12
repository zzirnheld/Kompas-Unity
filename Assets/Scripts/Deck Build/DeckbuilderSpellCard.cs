using KompasCore.Cards;

namespace KompasDeckbuilder
{
    public class DeckbuilderSpellCard : DeckbuilderCard
    {
        public int c;
        public string spellType;
        public bool fast;
        public string subtext;

        public string StatsString => $"D: {c}  Subtext: {subtext}";
        public override string BlurbString => $"C{c}{(fast ? " Fast" : "")} {SubtypeText}";

        public override void SetInfo(CardSearchController searchCtrl, SerializableCard spellCard, bool inDeck)
        {
            base.SetInfo(searchCtrl, spellCard, inDeck);
            c = spellCard.c;
            spellType = spellCard.spellType;
            fast = spellCard.fast;
            subtext = spellCard.subtext;
        }

        public override void Show()
        {
            base.Show();
            cardSearchController.StatsText.text = StatsString;
        }
    }
}