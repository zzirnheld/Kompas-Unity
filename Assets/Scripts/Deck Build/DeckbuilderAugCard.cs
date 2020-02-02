using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderAugCard : DeckbuilderCard
{
    public int d;
    public bool fast;
    public string subtext;
    public string[] augSubtypes;

    public string StatsString
    {
        get { return $"A: {d}  Subtext: {subtext}"; }
    }

    public void SetInfo(CardSearchController searchCtrl, SerializableAugCard augCard)
    {
        base.SetInfo(searchCtrl, augCard);
        d = augCard.d;
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
