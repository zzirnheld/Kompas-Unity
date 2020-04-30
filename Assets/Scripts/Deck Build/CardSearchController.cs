using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardSearchController : MonoBehaviour
{
    public const string cardBackPath = "Detailed Sprites/Square Kompas Logo";

    //card data pane ui elements
    public GameObject CardSearchPaneParentObj;
    public Image CardImage;
    public TMP_Text CardNameText;
    public TMP_Text StatsText;
    public TMP_Text SubtypesText;
    public TMP_Text EffectText;
    private Sprite CardBack;

    //card prefabs
    public GameObject CharPrefab;
    public GameObject SpellPrefab;
    public GameObject AugPrefab;

    //other scripts
    public DeckbuilderController DeckbuilderCtrl;
    public CardRepository CardRepo;

    //search data
    private DeckbuilderCard selectedCard;
    private List<DeckbuilderCard> shownCards;
    private string cardNameToSearch = "";
    private string subtypeToSearch = "";

    void Awake()
    {
        //initialize list
        shownCards = new List<DeckbuilderCard>();

        //get image to show when no card is selected
        CardBack = Resources.Load<Sprite>(cardBackPath);
        //show the blank card
        ShowSelectedCard();
    }

    /// <summary>
    /// Show the card currently selected instead of another one.
    /// </summary>
    public void ShowSelectedCard()
    {
        //if there is a selected card, show it
        if(selectedCard != null) selectedCard.Show();
        //otherwise, show data for no card, and show the card back as the sprite
        else
        {
            CardImage.sprite = CardBack;
            CardNameText.text = "";
            StatsText.text = "";
            SubtypesText.text = "";
            EffectText.text = "";
        }
    }

    public void Select(DeckbuilderCard card)
    {
        selectedCard = card;
        card.Show();
    }

    private void SearchCards()
    {
        //first clear all shown cards
        foreach (DeckbuilderCard card in shownCards)
        {
            Destroy(card.gameObject);
        }
        shownCards.Clear();

        //don't do anything if it's an invalid string to search with
        if (string.IsNullOrWhiteSpace(cardNameToSearch) && string.IsNullOrWhiteSpace(subtypeToSearch)) return;

        //assuming everything is valid, get all the jsons that fit that
        List<string> jsonsThatFit = CardRepo.GetJsonsThatFit(cardNameToSearch, subtypeToSearch);
        //for each of the jsons, add it to the shown cards to be added
        foreach (string json in jsonsThatFit)
        {
            DeckbuilderCard newCard = CardRepo.InstantiateDeckbuilderCard(json, this, CardSearchPaneParentObj.transform, false);
            if (newCard != null) shownCards.Add(newCard);
        }
    }

    /// <summary>
    /// Called when the name input field has its data changed
    /// </summary>
    /// <param name="name">The string that should be in the name</param>
    public void SearchCardsByName(string name)
    {
        cardNameToSearch = name.Replace("\u200B", "");
        SearchCards();
    }

    /// <summary>
    /// Called when the subtype input field has its data changed
    /// </summary>
    /// <param name="subtype">The string that should be in the subtype</param>
    public void SearchCardsBySubtype(string subtype)
    {
        subtypeToSearch = subtype.Replace("\u200B", "");
        SearchCards();
    }
}
