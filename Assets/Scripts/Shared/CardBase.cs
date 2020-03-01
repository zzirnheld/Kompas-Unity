using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class CardBase : MonoBehaviour
{
    public char CardType { get; private set; }
    public string CardName { get; private set; }
    public string EffText { get; private set; }
    public string SubtypeText { get; private set; }
    public string[] Subtypes { get; private set; }

    protected Sprite detailedSprite;
    protected Sprite simpleSprite;

    public virtual void SetInfo(SerializableCard card)
    {
        CardType = card.cardType;
        CardName = card.cardName;
        EffText = card.effText;
        SubtypeText = card.subtypeText;
        Subtypes = card.subtypes;
    }
}
