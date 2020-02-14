using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DeckbuilderCard : MonoBehaviour
{
    protected CardSearchController cardSearchController;

    public char CardType { get; private set; }
    public string CardName { get; private set; }
    public string EffText { get; private set; }
    public string SubtypeText { get; private set; }
    public string[] Subtypes { get; private set; }

    public bool InDeck;

    protected Image image;
    protected Sprite detailedSprite;
    protected Sprite simpleSprite;

    public void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetInfo(CardSearchController searchCtrl, SerializableCard card, bool inDeck)
    {
        CardType = card.cardType;
        cardSearchController = searchCtrl;
        CardName = card.cardName;
        SetImage(CardName);
        EffText = card.effText;
        SubtypeText = card.subtypeText;
        Subtypes = card.subtypes;
        InDeck = inDeck;
    }

    public void Select()
    {
        cardSearchController.Select(this);
    }

    /// <summary>
    /// Shows this card in the "selected card" area of the deckbuilder
    /// </summary>
    public virtual void Show()
    {
        cardSearchController.CardImage.sprite = detailedSprite;
        cardSearchController.CardNameText.text = CardName;
        cardSearchController.SubtypesText.text = SubtypeText;
        cardSearchController.EffectText.text = EffText;
    }

    public void Unshow()
    {
        cardSearchController.ShowSelectedCard();
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

    public void OnClick()
    {
        Debug.Log($"Clicked {CardName}, in deck? {InDeck}");
        if (InDeck)
        {
            cardSearchController.DeckbuilderCtrl.RemoveFromDeck(this);
        }
        else
        {
            Select();
            cardSearchController.DeckbuilderCtrl.AddToDeck(this);
        }
    }
}
