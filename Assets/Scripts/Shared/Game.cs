using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    public static Game mainGame;

    public enum TargetMode { Free, OnHold, BoardTarget, HandTarget, SpaceTarget }

    //other scripts
    public MouseController mouseCtrl;
    public NetworkController networkCtrl;
    public UIController uiCtrl;

    //game mechanics
    public BoardController boardCtrl;
    //game objects
    public GameObject boardObject;

    //list of card names 
    public static Dictionary<int, string> CardNames;
    public static Dictionary<string, int> CardNameIndices;

    public int turnPlayer = 0;
    protected Player[] players = new Player[2];
    public Player[] Players { get { return players; } }
    public Player TurnPlayer { get { return players[turnPlayer]; } }

    //game data
    public Dictionary<int, Card> cards;
    public int MaxCardsOnField = 0; //for pip generation purposes

    //The Stack
    protected List<IStackable> stack;
    protected int stackIndex;

    //trigger map
    protected Dictionary<TriggerCondition, List<Trigger>> triggerMap;

    protected Effect currEffect;
    public Effect CurrEffect
    {
        get => currEffect;
        set => currEffect = value;
    }

    public TargetMode targetMode = TargetMode.Free;

    private void Start()
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
            Debug.Log("Adding \"" + cardNames[i] + "\", length " + cardNames[i].Length);
            CardNames.Add(i, toAdd); //because line endings
            CardNameIndices.Add(toAdd, i);
        }

        stackIndex = -1;
    }

    public Card GetCardFromID(int id)
    {
        Debug.Log("Getting card with id " + id + " is it in the dictionary? " + cards.ContainsKey(id));

        if (!cards.ContainsKey(id)) return null;

        return cards[id];
    }

    public Card Draw(int player = 0)
    {
        Card toDraw = players[player].deckCtrl.PopTopdeck();
        toDraw.Rehand(player);
        return toDraw;
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
    public bool ExistsDeckTarget(CardRestriction restriction, int player)
    {
        return players[player].deckCtrl.Exists(restriction);
    }

    public bool ExistsDiscardTarget(CardRestriction cardRestriction, int player)
    {
        return players[player].discardCtrl.Exists(cardRestriction);
    }

    public bool ExistsHandTarget(CardRestriction cardRestriction, int player)
    {
        return players[player].handCtrl.Exists(cardRestriction);
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

}
