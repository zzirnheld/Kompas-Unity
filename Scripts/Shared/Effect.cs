using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Effect
{
    public Card thisCard;
    public int EffectController;

    private Subeffect[] subeffects;

    public void Resolve()
    {

    }
}
