using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderSpellCard : DeckbuilderCard
{
    public int d;
    public SpellCard.SpellType subtype;
    public bool fast;
    public string subtext;

    public void SetInfo(SerializableSpellCard spellCard)
    {
        base.SetInfo(spellCard);
        d = spellCard.d;
        subtype = spellCard.subtype;
        fast = spellCard.fast;
        subtext = spellCard.subtext;
    }
}
