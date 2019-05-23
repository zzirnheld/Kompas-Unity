using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClientUIController : UIController
{
    ClientGame clientGame;
    //deck importing
    public InputField deckInputField;
    public Button importDeckButton;
    public Button confirmDeckImportButton;
    //card search
    public GameObject cardSearchView;
    public Image cardSearchImage;
    public Button deckSearchButton;
    public Button discardSearchButton;
    public Button searchDeckToHand;
    public Button searchDiscardToHand;
    public Button cancelDeckSearch;
    public Button cancelDiscardSearch;
    private bool searchingDeck = false;
    private bool searchingDiscard = false;
    private int searchIndex = 0;
    
    private void Awake()
    {
        deckInputField.lineType = InputField.LineType.MultiLineNewline;
    }

    public override void SelectCard(Card card, Game.TargetMode targetMode, bool fromClick)
    {
        base.SelectCard(card, targetMode, fromClick);
        if (fromClick) clientGame.TargetCard(card);
    }

    public void ActivateSelectedCardEff(int index)
    {
        if (selectedCard != null)
            clientGame.clientNetworkCtrl.RequestResolveEffect(selectedCard, index);
    }


    #region importing/searching deck or discard
    public void ImportDeckPressed()
    {
        deckInputField.gameObject.SetActive(true);
        importDeckButton.gameObject.SetActive(false);
        confirmDeckImportButton.gameObject.SetActive(true);
    }

    public void ConfirmDeckImport()
    {
        string decklist = deckInputField.text;
        deckInputField.gameObject.SetActive(false);
        confirmDeckImportButton.gameObject.SetActive(false);
        importDeckButton.gameObject.SetActive(true);
        //TODO change this to ask the server for import deck
        //if(Game.DEBUG_MODE) clientGame.friendlyDeckCtrl.ImportDeck(decklist);
        clientGame.clientNetworkCtrl.RequestDecklistImport(decklist);
    }

    //TODO assign all of these methods to buttons
    //TODO rework this to take into get deck from server before searching
    public void StartDeckSearch()
    {
        if (searchingDiscard) return;
        searchingDeck = true;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(false);
        deckSearchButton.gameObject.SetActive(false);
        searchDeckToHand.gameObject.SetActive(true);
        cancelDeckSearch.gameObject.SetActive(true);
        //initiate search process
        searchIndex = 0;
        if (clientGame.friendlyDeckCtrl.DeckSize() > 0)
            cardSearchImage.sprite = clientGame.friendlyDeckCtrl.CardAt(0, false, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDeckToHand()
    {
        if (!searchingDeck) return;
        //and request get the card you're currently at for searching
        clientGame.clientNetworkCtrl.RequestRehand(clientGame.friendlyDeckCtrl.CardAt(searchIndex, false));
        //then be done searching
        EndDeckSearch();
    }

    public void EndDeckSearch()
    {
        cardSearchView.SetActive(false);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(true);
        deckSearchButton.gameObject.SetActive(true);
        searchDeckToHand.gameObject.SetActive(false);
        cancelDeckSearch.gameObject.SetActive(false);
        searchingDeck = false;
    }

    public void StartDiscardSearch()
    {
        if (searchingDeck) return;
        searchingDiscard = true;
        cardSearchView.SetActive(true);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(false);
        deckSearchButton.gameObject.SetActive(false);
        searchDiscardToHand.gameObject.SetActive(true);
        cancelDiscardSearch.gameObject.SetActive(true);
        //intiate process of searching
        searchIndex = 0;
        if (clientGame.friendlyDiscardCtrl.DiscardSize() > 0)
            cardSearchImage.sprite = clientGame.friendlyDiscardCtrl.CardAt(0, false).DetailedSprite;
        else
            cardSearchImage.sprite = Resources.Load<Sprite>("Card Sprites/Square Kompas Logo");
    }

    public void SearchDiscardToHand()
    {
        if (!searchingDiscard) return;
        //request to add the card that you've searched for
        clientGame.clientNetworkCtrl.RequestRehand(clientGame.friendlyDiscardCtrl.CardAt(searchIndex, false));
        //and end the search
        EndDiscardSearch();
    }

    public void EndDiscardSearch()
    {
        cardSearchView.SetActive(false);
        //set buttons to their correct states
        discardSearchButton.gameObject.SetActive(true);
        deckSearchButton.gameObject.SetActive(true);
        searchDiscardToHand.gameObject.SetActive(false);
        cancelDiscardSearch.gameObject.SetActive(false);
        //intiate process of searching
        searchingDiscard = false;
    }

    public void NextCardSearch()
    {
        searchIndex++;
        if (searchingDeck)
        {
            if (clientGame.friendlyDeckCtrl.DeckSize() > 0)
            {
                searchIndex %= clientGame.friendlyDeckCtrl.DeckSize();
                cardSearchImage.sprite = clientGame.friendlyDeckCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
        else if (searchingDiscard)
        {
            if (clientGame.friendlyDiscardCtrl.DiscardSize() > 0)
            {
                searchIndex %= clientGame.friendlyDiscardCtrl.DiscardSize();
                cardSearchImage.sprite = clientGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
            }
        }
    }

    public void PrevCardSearch()
    {
        searchIndex--;
        if (searchingDeck)
        {
            if (searchIndex < 0) searchIndex += clientGame.friendlyDeckCtrl.DeckSize();
            if (clientGame.friendlyDeckCtrl.DeckSize() > 0)
                cardSearchImage.sprite = clientGame.friendlyDeckCtrl.CardAt(searchIndex, false, false).DetailedSprite;
        }
        else if (searchingDiscard)
        {
            if (searchIndex < 0) searchIndex += clientGame.friendlyDiscardCtrl.DiscardSize();
            if (clientGame.friendlyDiscardCtrl.DiscardSize() > 0)
                cardSearchImage.sprite = clientGame.friendlyDiscardCtrl.CardAt(searchIndex, false).DetailedSprite;
        }
    }
    #endregion
}
