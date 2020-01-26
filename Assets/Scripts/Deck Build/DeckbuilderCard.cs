using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DeckbuilderCard : MonoBehaviour, IPointerEnterHandler
{
    private CardSearch CardSearchController;

    public char CardType;
    public string CardName;
    public string EffText;
    public string SubtypeText;
    public string[] Subtypes;

    protected Image image;
    protected Sprite detailedSprite;
    protected Sprite simpleSprite;

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetInfo(CardSearch searchCtrl, SerializableCard card)
    {
        CardType = card.cardType;
        CardSearchController = searchCtrl;
        CardName = card.cardName;
        SetImage(CardName);
        EffText = card.effText;
        SubtypeText = card.subtypeText;
        Subtypes = card.subtypes;
    }

    /// <summary>
    /// Shows this card in the "selected card" area of the deckbuilder
    /// </summary>
    public void Show()
    {
        CardSearchController.CardImage.sprite = detailedSprite;
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
        image.sprite = simpleSprite;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Show();
    }
}
