using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckbuilderCharCard : DeckbuilderCard
{
    public int n;
    public int e;
    public int s;
    public int w;

    public void SetInfo(SerializableCharCard charCard)
    {
        base.SetInfo(charCard);
        n = charCard.n;
        e = charCard.e;
        s = charCard.s;
        w = charCard.w;
    }
}
