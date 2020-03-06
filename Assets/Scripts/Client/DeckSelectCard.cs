using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelectCard : CardBase
{
    public Image Image;

    public new void SetInfo(SerializableCard card)
    {
        base.SetInfo(card);
        SetImage(CardName);
    }

    protected void SetImage(string cardFileName)
    {
        detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + cardFileName);
        simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
        //check if either is null. if so, log to debug and return
        if (detailedSprite == null || simpleSprite == null)
        {
            Debug.LogError("Could not find sprite with name " + cardFileName);
            return;
        }
        Image.sprite = simpleSprite;
    }
}
