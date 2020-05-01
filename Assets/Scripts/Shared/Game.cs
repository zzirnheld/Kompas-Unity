using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;

public abstract class Game : MonoBehaviour {

    public static Game mainGame;

    public enum TargetMode { Free, OnHold, BoardTarget, HandTarget, SpaceTarget }

    //other scripts
    public UIController uiCtrl;

    //game mechanics
    public BoardController boardCtrl;
    //game objects
    public GameObject boardObject;

    //list of card names 
    public static Dictionary<int, string> CardNames;
    public static Dictionary<string, int> CardNameIndices;
    public CardRepository CardRepo;

    public abstract Player[] Players { get; }
    public int turnPlayer = 0;
    public Player TurnPlayer { get { return Players[turnPlayer]; } }

    //game data
    public Dictionary<int, Card> cards;
    public int MaxCardsOnField = 0; //for pip generation purposes
    public ISubeffectFactory SubeffectFactory;

    //trigger map
    protected Dictionary<TriggerCondition, List<ServerTrigger>> triggerMap;
    protected Dictionary<TriggerCondition, List<HangingEffect>> hangingEffectMap;
    
    public ServerEffect CurrEffect { get; set; }

    public TargetMode targetMode = TargetMode.Free;

    private void Awake()
    {
        cards = new Dictionary<int, Card>();
        CardNames = new Dictionary<int, string>();
        CardNameIndices = new Dictionary<string, int>();
        string cardListPath = "Card Jsons/Card List";
        string cardList = Resources.Load<TextAsset>(cardListPath).text;
        cardList = cardList.Replace('\r', '\n');
        string[] cardNames = cardList.Split('\n');
        
        for(int i = 0; i < cardNames.Length; i++)
        {
            string toAdd = cardNames[i].Substring(0, cardNames[i].Length);
            if (CardNameIndices.ContainsKey(toAdd)) continue;
            //Debug.Log("Adding \"" + cardNames[i] + "\", length " + cardNames[i].Length);
            CardNames.Add(i, toAdd); //because line endings
            CardNameIndices.Add(toAdd, i);
        }
        
        triggerMap = new Dictionary<TriggerCondition, List<ServerTrigger>>();
        foreach (TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            triggerMap.Add(c, new List<ServerTrigger>());
        }

        hangingEffectMap = new Dictionary<TriggerCondition, List<HangingEffect>>();
        foreach (TriggerCondition c in System.Enum.GetValues(typeof(TriggerCondition)))
        {
            hangingEffectMap.Add(c, new List<HangingEffect>());
        }
    }

    public virtual void OnClickBoard(int x, int y) { }
    public virtual void Lose(int controllerIndex) { }

    public Card GetCardFromID(int id)
    {
        Debug.Log("Getting card with id " + id + " is it in the dictionary? " + cards.ContainsKey(id));

        if (!cards.ContainsKey(id)) return null;

        return cards[id];
    }

    //game mechanics
    //checking for valid target
    public bool ExistsValidCardOnBoardTarget(CardRestriction restriction)
    {
        return boardCtrl.ExistsCardOnBoard(restriction);
    }

    public bool NoValidCardOnBoardTarget(CardRestriction restriction)
    {
        return !boardCtrl.ExistsCardOnBoard(restriction);
    }

    /// <summary>
    /// Checks if there exists a target in one player's deck that fits a given restriction.
    /// </summary>
    /// <param name="restriction">The restriction a card must fit</param>
    /// <param name="player">The player index whose deck to look for cards in </param>
    /// <returns></returns>
    public bool ExistsDeckTarget(CardRestriction restriction, Player player)
    {
        return player.deckCtrl.Exists(restriction);
    }

    public bool ExistsDiscardTarget(CardRestriction cardRestriction, Player player)
    {
        return player.discardCtrl.Exists(cardRestriction);
    }

    public bool ExistsHandTarget(CardRestriction cardRestriction, Player player)
    {
        return player.handCtrl.Exists(cardRestriction);
    }

    public bool ExistsSpaceTarget(SpaceRestriction restriction)
    {
        for(int x = 0; x < 7; x++)
        {
            for(int y = 0; y < 7; y++)
            {
                if (restriction.Evaluate(x, y)) return true;
            }
        }

        return false;
    }

    #region move card between areas
    //so that notify stuff can be sent in the server
    private void Remove(Card card)
    {
        switch (card.Location)
        {
            case CardLocation.Field:
                boardCtrl.RemoveFromBoard(card);
                break;
            case CardLocation.Discard:
                card.Controller.discardCtrl.RemoveFromDiscard(card);
                break;
            case CardLocation.Hand:
                card.Controller.handCtrl.RemoveFromHand(card);
                break;
            case CardLocation.Deck:
                card.Controller.deckCtrl.RemoveFromDeck(card);
                break;
            default:
                Debug.LogWarning($"Tried to remove card from invalid location {card.Location}");
                break;
        }
    }

    public void Discard(Card card, IStackable stackSrc = null)
    {
        Remove(card);
        card.Controller.discardCtrl.AddToDiscard(card);
    }

    public void Rehand(Player controller, Card card, IStackable stackSrc = null)
    {
        Remove(card);
        //let the card know whose hand it'll be added
        card.ChangeController(controller);
        controller.handCtrl.AddToHand(card);
    }

    public void Rehand(Card card, IStackable stackSrc = null)
    {
        Rehand(card.Controller, card);
    }

    public void Reshuffle(Card card, IStackable stackSrc = null)
    {
        Remove(card);
        card.Controller.deckCtrl.ShuffleIn(card);
    }

    public void Topdeck(Card card, IStackable stackSrc = null)
    {
        Remove(card);
        card.Controller.deckCtrl.PushTopdeck(card);
    }

    public void Bottomdeck(Card card, IStackable stackSrc = null)
    {
        Remove(card);
        card.Controller.deckCtrl.PushBottomdeck(card);
    }

    public void Play(Card card, int toX, int toY, Player controller, IStackable stackSrc = null)
    {
        Remove(card);
        boardCtrl.Play(card, toX, toY, controller);
        card.ChangeController(controller);
    }

    public void MoveOnBoard(Card card, int toX, int toY, IStackable stackSrc = null)
    {
        boardCtrl.Move(card, toX, toY);
    }

    public void Negate(Card c, IStackable stackSrc = null)
    {
        c.Negate();
    }
    #endregion move card between areas

    public virtual void SetStats(SpellCard spellCard, int c)
    {
        if(spellCard == null)
        {
            Debug.LogError("Tried to set stats on null spell card");
            return;
        }
        spellCard.C = c;
    }

    public virtual void SetStats(CharacterCard charCard, int n, int e, int s, int w)
    {
        if(charCard == null)
        {
            Debug.LogError("Tried to set stats on null character card");
            return;
        }
        charCard.SetNESW(n, e, s, w);
    }

    public void SetStats(CharacterCard charCard, int[] stats)
    {
        if (stats.Length != 4) throw new System.ArgumentException("Stats array length must be 4");

        SetStats(charCard, stats[0], stats[1], stats[2], stats[3]);
    }

    public void AddToStats(CharacterCard charCard, int nMod, int eMod, int sMod, int wMod)
    {
        SetStats(charCard,
            charCard.N + nMod,
            charCard.E + eMod,
            charCard.S + sMod,
            charCard.W + wMod);
    }

    public void SwapStats(CharacterCard a, CharacterCard b, bool swapN = true, bool swapE = true, bool swapS = true, bool swapW = true)
    {
        int[] aNewStats = new int[4];
        int[] bNewStats = new int[4];

        (aNewStats[0], bNewStats[0]) = swapN ? (b.N, a.N) : (a.N, b.N);
        (aNewStats[1], bNewStats[1]) = swapE ? (b.E, a.E) : (a.E, b.E);
        (aNewStats[2], bNewStats[2]) = swapS ? (b.S, a.S) : (a.S, b.S);
        (aNewStats[3], bNewStats[3]) = swapW ? (b.W, a.W) : (a.W, b.W);

        SetStats(a, aNewStats[0], aNewStats[1], aNewStats[2], aNewStats[3]);
        SetStats(b, bNewStats[0], bNewStats[1], bNewStats[2], bNewStats[3]);
    }

    public virtual void Dispel(SpellCard spell)
    {
        Negate(spell);
        Discard(spell);
    }

    #region triggers
    public void RegisterTrigger(TriggerCondition condition, ServerTrigger trigger)
    {
        Debug.Log($"Registering a new trigger from card {trigger.effToTrigger.thisCard.CardName} to condition {condition}");
        List<ServerTrigger> triggers = triggerMap[condition];
        if (triggers == null)
        {
            triggers = new List<ServerTrigger>();
            triggerMap.Add(condition, triggers);
        }
        triggers.Add(trigger);
    }

    public void RegisterHangingEffect(TriggerCondition condition, HangingEffect hangingEff)
    {
        Debug.Log($"Registering a new hanging effect to condition {condition}");
        List<HangingEffect> hangingEffs = hangingEffectMap[condition];
        hangingEffs.Add(hangingEff);
    }
    #endregion
}
