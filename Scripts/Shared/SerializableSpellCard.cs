using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableSpellCard : SerializableCard {

    public int d;
    public SpellCard.SpellType subtype;
    public bool fast;
    public string subtext;

}
