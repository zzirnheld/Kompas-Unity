using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckController : MonoBehaviour, KompasObject
{
    public const string BLANK_CARD_PATH = "Card Jsons/Blank Card";

    public Game game;
    public Game Game { get => game; set => game = value; }

    private ClientGame clientGame;
    private ServerGame serverGame;

    //one of these for each player
    public Player Owner;

    //rng for shuffling
    private static System.Random rng = new System.Random();

    //prefabs to instantiate in deck
    public GameObject characterCardPrefab;
    public GameObject spellCardPrefab;
    public GameObject augmentCardPrefab;

    //actual factual deck list
    private List<Card> deck = new List<Card>();
    public List<Card> Deck { get { return deck; } }

    private void Awake()
    {
        clientGame = game as ClientGame;
        serverGame = game as ServerGame;
    }

    public int IndexOf(Card card)
    {
        return deck.IndexOf(card);
    }

    //importing deck
    public Card InstantiateCard(string json, Player owner)
    {
        //already allocated serializable cards
        SerializableCharCard serializableChar;
        SerializableSpellCard serializableSpell;
        SerializableAugCard serializableAug;
        SerializableCard serializableCard;
        //and non serializable cards
        CharacterCard charCard;
        SpellCard spellCard;
        AugmentCard augCard;

        //first deserialize it to tell the card's type
        serializableCard = JsonUtility.FromJson<SerializableCard>(json);
        switch (serializableCard.cardType)
        {
            case 'C':
                serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                charCard = Instantiate(characterCardPrefab).GetComponent<CharacterCard>();
                charCard.gameObject.SetActive(false);
                charCard.SetInfo(serializableChar, game, owner);
                //set image for the card by the name. this method gets the sprite with the given name
                charCard.SetImage(charCard.CardName);
                return charCard;
            case 'S':
                serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                spellCard = Instantiate(spellCardPrefab).GetComponent<SpellCard>();
                spellCard.gameObject.SetActive(false);
                spellCard.SetInfo(serializableSpell, game, owner);
                //set image for the card by the name. this method gets the sprite with the given name
                spellCard.SetImage(spellCard.CardName);
                return spellCard;
            case 'A':
                serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                augCard = Instantiate(augmentCardPrefab).GetComponent<AugmentCard>();
                augCard.gameObject.SetActive(false);
                augCard.SetInfo(serializableAug, game, owner);
                //set image for the card by the name. this method gets the sprite with the given name
                augCard.SetImage(augCard.CardName);
                return augCard;
            default:
                Debug.Log("Unrecognized type character " + serializableCard.cardType + " in " + json);
                return null;
        }
    }

    public Card InstantiateBlankCard(Player owner)
    {
        return InstantiateCard(Resources.Load<TextAsset>(BLANK_CARD_PATH).text, owner);
    }

    public Card AddBlankCard()
    {
        Card blank = InstantiateBlankCard(Owner);
        PushTopdeck(blank);
        return blank;
    }

    public Card AddCard(string cardName, int id, Player owner)
    {
        Card newCard;
        string fileContents = game.CardRepo.GetJsonFromName(cardName);

        Debug.Log("Loading:\n" + fileContents);

        newCard = InstantiateCard(fileContents, owner);
        newCard.SetLocation(CardLocation.Deck);
        deck.Add(newCard);
        newCard.ID = id;
        game.cards.Add(id, newCard);
        //Game.mainGame.cards[id] = newCard;
        newCard.ChangeController(owner);

        if (serverGame != null)
        {
            foreach (Effect eff in newCard.Effects)
            {
                if (eff.Trigger != null)
                {
                    Debug.Log("registering trigger for " + eff.Trigger.triggerCondition);
                    serverGame.RegisterTrigger(eff.Trigger.triggerCondition, eff.Trigger);
                }
                else Debug.Log("trigger is null");
            }
        }
        return newCard;
    }

    //info about deck
    public int DeckSize() { return deck.Count; }
    
    /// <summary>
    /// Gets the card at the designated index.
    /// </summary>
    /// <param name="index">Index of the card to get</param>
    /// <param name="remove">Whether or not to remove the card</param>
    /// <param name="shuffle">Whether or not to shuffle the deck after getting the card</param>
    /// <returns></returns>
    public Card CardAt(int index, bool remove, bool shuffle = false)
    {
        if (index > deck.Count) return null;
        Card card = deck[index];
        if (remove) deck.RemoveAt(index);
        if (shuffle) Shuffle();
        return card;
    }

    //adding and removing cards
    public void PushTopdeck(Card card)
    {
        deck.Insert(0, card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
    }

    public void PushBottomdeck(Card card)
    {
        deck.Add(card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
    }

    public void ShuffleIn(Card card)
    {
        deck.Add(card);
        card.ResetCard();
        card.SetLocation(CardLocation.Deck);
        Shuffle();
    }

    public Card PopTopdeck()
    {
        if (deck.Count == 0) return null;

        Card card = deck[0];
        deck.RemoveAt(0);
        return card;
    }

    public Card PopBottomdeck()
    {
        if (deck.Count == 0) return null;

        Card card = deck[deck.Count - 1];
        deck.RemoveAt(deck.Count - 1);
        return card;
    }

    public Card RemoveCardWithName(string name)
    {
        Card toReturn;
        for(int i = 0; i < deck.Count; i++)
        {
            if (deck[i].CardName.Equals(name))
            {
                toReturn = deck[i];
                deck.RemoveAt(i);
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
        deck.Remove(card);
    }

    //misc
    public void Shuffle()
    {
        int n = deck.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Card value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public void OnClick()
    {
        //if(deck.Count > 0) Game.mainGame.Draw();
        //request a draw
        if(clientGame.friendlyDeckCtrl == this)
            clientGame.clientNotifier.RequestDraw();
    }

    /// <summary>
    /// Checks if a card exists that fits the given restriction
    /// </summary>
    /// <param name="cardRestriction"></param>
    /// <returns></returns>
    public bool Exists(CardRestriction cardRestriction)
    {
        foreach(Card c in deck)
        {
            if (c != null && cardRestriction.Evaluate(c)) return true;
        }

        return false;
    }

    public List<Card> CardsThatFitRestriction(CardRestriction cardRestriction)
    {
        List<Card> cards = new List<Card>();
        foreach(Card c in deck)
        {
            if (c != null && cardRestriction.Evaluate(c))
                cards.Add(c);
        }
        return cards;
    }

    public void OnHover() { }

    public void OnDrag(Vector3 mousePos) { }

    public void OnDragEnd(Vector3 mousePos) { }
}
