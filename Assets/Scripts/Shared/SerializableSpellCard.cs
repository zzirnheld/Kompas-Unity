using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SerializableSpellCard : SerializableCard {

    public int c;
    public SpellCard.SpellType spellType;
    public bool fast;
    public string subtext;

}
