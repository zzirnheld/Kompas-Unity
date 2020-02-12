using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardSearchController : MonoBehaviour
{
    public const string cardBackPath = "Detailed Sprites/Square Kompas Logo";

    public GameObject CardSearchPaneParentObj;
    public TMP_Text CardSearchName;
    public GameObject CharPrefab;
    public GameObject SpellPrefab;
    public GameObject AugPrefab;

    public Image CardImage;
    public TMP_Text CardNameText;
    public TMP_Text StatsText;
    public TMP_Text SubtypesText;
    public TMP_Text EffectText;

    private Sprite CardBack;

    private DeckbuilderCard selectedCard;
    public DeckbuilderCard SelectedCard { get { return selectedCard; } }

    protected List<DeckbuilderCard> shownCards;

    public DeckbuilderController DeckbuilderCtrl;
    public CardRespository CardRepo;

    private string cardNameToSearch = "";
    private string subtypeToSearch = "";

    void Awake()
    {
        shownCards = new List<DeckbuilderCard>();

        CardBack = Resources.Load<Sprite>(cardBackPath);

        ShowSelectedCard();
    }

    /// <summary>
    /// Show the card currently selected instead of another one.
    /// </summary>
    public void ShowSelectedCard()
    {
        if(selectedCard != null) selectedCard.Show();
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
        foreach (DeckbuilderCard card in shownCards)
        {
            Destroy(card.gameObject);
        }

        shownCards.Clear();
        if (string.IsNullOrWhiteSpace(cardNameToSearch) && string.IsNullOrWhiteSpace(subtypeToSearch)) return;

        //for now, only search by name
        /*Debug.Log($"Search cards called for \"{cardNameToSearchFor}\", length {cardNameToSearchFor.Length}, first char" +
            $"{(int) cardNameToSearchFor[0]} aka \"{cardNameToSearchFor[0]}\"");*/
        List<string> jsonsThatFit = CardRepo.GetJsonsThatFit(cardNameToSearch, subtypeToSearch);

        foreach (string json in jsonsThatFit)
        {
            DeckbuilderCard newCard = CardRepo.InstantiateCard(json, CardSearchPaneParentObj.transform, false);
            if (newCard != null) shownCards.Add(newCard);
        }
    }

    public void SearchCardsByName(string name)
    {
        cardNameToSearch = name.Replace("\u200B", "");
        SearchCards();
    }

    public void SearchCardsBySubtype(string subtype)
    {
        subtypeToSearch = subtype.Replace("\u200B", "");
        Debug.Log($"Searching for subtype \"{subtype}\"");
        SearchCards();
    }
}
