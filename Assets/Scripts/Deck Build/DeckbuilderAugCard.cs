using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderAugCard : DeckbuilderCard
{
    public int a;
    public bool fast;
    public string subtext;
    public string[] augSubtypes;

    public string StatsString
    {
        get { return $"A: {a}  Subtext: {subtext}"; }
    }

    public override void SetInfo(CardSearchController searchCtrl, SerializableCard augCard, bool inDeck)
    {
        base.SetInfo(searchCtrl, augCard, inDeck);
        a = augCard.a;
        augSubtypes = augCard.augSubtypes;
        fast = augCard.fast;
        subtext = augCard.subtext;
    }

    public override void Show()
    {
        base.Show();
        cardSearchController.StatsText.text = StatsString;
    }
}
