using KompasCore.Cards;

namespace KompasDeckbuilder
{
    public class DeckbuilderCharCard : DeckbuilderCard
    {
        public int n;
        public int e;
        public int s;
        public int w;

        public string StatsString => $"N: {n}  E: {e}  S: {s}  W: {w}";
        public override string BlurbString => $"N{n} / E{e} / S{s} / W{w} {SubtypeText}";

        public override void SetInfo(CardSearchController searchCtrl, SerializableCard charCard, bool InDeck)
        {
            base.SetInfo(searchCtrl, charCard, InDeck);
            n = charCard.n;
            e = charCard.e;
            s = charCard.s;
            w = charCard.w;
        }

        public override void Show()
        {
            base.Show();
            cardSearchController.StatsText.text = StatsString;
        }
    }
}