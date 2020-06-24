using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;
using System.Linq;

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
    public int TurnPlayerIndex { get; protected set; } = 0;
    public Player TurnPlayer { get { return Players[TurnPlayerIndex]; } }

    //game data
    public abstract IEnumerable<GameCard> Cards { get; }
    public int maxCardsOnField = 0; //for pip generation purposes
    public virtual int Leyload
    {
        get => maxCardsOnField;
        set => maxCardsOnField = value;
    }
    
    public ServerEffect CurrEffect { get; set; }

    public TargetMode targetMode = TargetMode.Free;

    private void Awake()
    {
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
    }

    public virtual void OnClickBoard(int x, int y) { }
    public virtual void Lose(int controllerIndex) { }

    public abstract GameCard GetCardWithID(int id);

    //game mechanics
    //checking for valid target

    public bool ExistsBoardTarget(CardRestriction restriction)
    {
        return Cards.Any(c => c.Location == CardLocation.Field && restriction.Evaluate(c));
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
}
