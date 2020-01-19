using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DeckbuilderCard : MonoBehaviour
{
    public string cardName;
    public string effText;
    public string subtypeText;
    public string[] subtypes;

    public void SetInfo(SerializableCard card)
    {
        cardName = card.cardName;
        effText = card.effText;
        subtypeText = card.subtypeText;
        subtypes = card.subtypes;
    }
}
