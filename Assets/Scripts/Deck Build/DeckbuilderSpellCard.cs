using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderSpellCard : DeckbuilderCard
{
    public int c;
    public string spellType;
    public bool fast;
    public string subtext;

    public string StatsString
    {
        get { return $"D: {c}  Subtext: {subtext}"; }
    }

    public void SetInfo(CardSearchController searchCtrl, SerializableSpellCard spellCard, bool inDeck)
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
