using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DeckController : MonoBehaviour
{
    public const string BLANK_CARD_PATH = "Card Jsons/Blank Card";

    public Game game;

    private ClientGame clientGame;
    private ServerGame serverGame;

    //one of these for each player
    public Player Owner;

    //rng for shuffling
    private static readonly System.Random rng = new System.Random();

    //prefabs to instantiate in deck
    public GameObject characterCardPrefab;
    public GameObject spellCardPrefab;
    public GameObject augmentCardPrefab;

    public List<Card> Deck { get; } = new List<Card>();

    private void Awake()
    {
        clientGame = game as ClientGame;
        serverGame = game as ServerGame;
    }

    public int IndexOf(Card card)
    {
        return Deck.IndexOf(card);
    }

    public void AddCard(Card card, int id)
    {
        card.SetLocation(CardLocation.Deck);
        Deck.Add(card);
        card.ID = id;
        game.cards.Add(id, card);
        card.ChangeController(Owner);
    }

    //info about deck
    public int DeckSize() { return Deck.Count; }
    
    /// <summary>
    /// Gets the card at the designated index.
    /// </summary>
    /// <param name="index">Index of the card to get</param>
    /// <param name="remove">Whether or not to remove the card</param>
    /// <param name="shuffle">Whether or not to shuffle the deck after getting the card</param>
    /// <returns></returns>
    public Card CardAt(int index, bool remove, bool shuffle = false)
    {
        if (index > Deck.Count) return null;
        Card card = Deck[index];
        if (remove) Deck.RemoveAt(index);
        if (shuffle) Shuffle();
        return card;
    }

    //adding and removing cards
    public void PushTopdeck(Card card)
    {
        Deck.Insert(0, card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
    }

    public void PushBottomdeck(Card card)
    {
        Deck.Add(card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
    }

    public void ShuffleIn(Card card)
    {
        Deck.Add(card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
        Shuffle();
    }

    public Card PopTopdeck()
    {
        if (Deck.Count == 0) return null;

        Card card = Deck[0];
        Deck.RemoveAt(0);
        return card;
    }

    public Card PopBottomdeck()
    {
        if (Deck.Count == 0) return null;

        Card card = Deck[Deck.Count - 1];
        Deck.RemoveAt(Deck.Count - 1);
        return card;
    }

    public Card RemoveCardWithName(string name)
    {
        Card toReturn;
        for(int i = 0; i < Deck.Count; i++)
        {
            if (Deck[i].CardName.Equals(name))
            {
                toReturn = Deck[i];
                Deck.RemoveAt(i);
                return toReturn;
            }
        }
        return null;
    }

    /// <summary>
    /// Random access remove from deck
    /// </summary>
    public void RemoveFromDeck(Card card)
    {
        Deck.Remove(card);
    }

    //misc
    public void Shuffle()
    {
        int n = Deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = Deck[k];
            Deck[k] = Deck[n];
            Deck[n] = value;
        }
    }

    /// <summary>
    /// Checks if a card exists that fits the given restriction
    /// </summary>
    /// <param name="cardRestriction"></param>
    /// <returns></returns>
    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(Card c in Deck)
        {
            if (c != null && cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }

    public List<Card> CardsThatFitRestriction(CardRestriction cardRestriction)
    {
        List<Card> cards = new List<Card>();
        foreach(Card c in Deck)
        {
            if (c != null && cardRestriction.Evaluate(c))
                cards.Add(c);
        }
        return cards;
    }

    public void OnMouseDown()
    {
        //request a draw
        if (clientGame.friendlyDeckCtrl == this)
            clientGame.clientNotifier.RequestDraw();
    }
}
