﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderAugCard : DeckbuilderCard
{
    public int d;
    public bool fast;
    public string subtext;
    public string[] augSubtypes;

    public void SetInfo(CardSearch searchCtrl, SerializableAugCard augCard)
    {
        base.SetInfo(searchCtrl, augCard);
        d = augCard.d;
        augSubtypes = augCard.augSubtypes;
        fast = augCard.fast;
        subtext = augCard.subtext;
    }
}
