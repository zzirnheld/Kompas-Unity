using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class DeckbuilderCard : MonoBehaviour
{
    protected CardSearchController cardSearchController;

    private char cardType;
    private string cardName;
    private string effText;
    private string subtypeText;
    private string[] subtypes;

    public char CardType { get { return cardType; } }
    public string CardName { get { return cardName; } }
    public string EffText { get { return effText; } }
    public string SubtypeText { get { return subtypeText; } }
    public string[] Subtypes { get { return subtypes; } }

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
        cardType = card.cardType;
        cardSearchController = searchCtrl;
        cardName = card.cardName;
        SetImage(CardName);
        effText = card.effText;
        subtypeText = card.subtypeText;
        subtypes = card.subtypes;
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
        Debug.Log($"Clicked {cardName}, in deck? {InDeck}");
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
