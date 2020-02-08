using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderSpellCard : DeckbuilderCard
{
    public int d;
    public SpellCard.SpellType subtype;
    public bool fast;
    public string subtext;

    public string StatsString
    {
        get { return $"D: {d}  Subtext: {subtext}"; }
    }

    public void SetInfo(CardSearchController searchCtrl, SerializableSpellCard spellCard, bool inDeck)
    {
        base.SetInfo(searchCtrl, spellCard, inDeck);
        d = spellCard.d;
        subtype = spellCard.subtype;
        fast = spellCard.fast;
        subtext = spellCard.subtext;
    }

    public override void Show()
    {
        base.Show();
        cardSearchController.StatsText.text = StatsString;
    }
}
