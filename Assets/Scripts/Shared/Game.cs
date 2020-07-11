using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasNetworking;
using System.Linq;

public abstract class Game : MonoBehaviour
{
    public const string CardListPath = "Card Jsons/Card List";

    public static Game mainGame;

    public enum TargetMode { Free, OnHold, BoardTarget, HandTarget, SpaceTarget }

    //other scripts
    public UIController uiCtrl;

    //game mechanics
    public BoardController boardCtrl;
    public AnnihilationController AnnihilationCtrl;
    //game objects
    public GameObject boardObject;

    //list of card names 
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

    public TargetMode targetMode = TargetMode.Free;

    public virtual void OnClickBoard(int x, int y) { }
    public virtual void Lose(int controllerIndex) { }

    public abstract GameCard GetCardWithID(int id);

    public virtual IStackable CurrStackEntry => null;

    //game mechanics
    //checking for valid target

    public bool ExistsCardTarget(CardRestriction restriction) => Cards.Any(c => restriction.Evaluate(c));

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
    
    private bool IsFriendlyAdjacentToCoords(int x, int y, GameCard potentialFriendly, Player friendly)
    {
        return boardCtrl.GetCardAt(x, y) == null
            && potentialFriendly != null && potentialFriendly.IsAdjacentTo(x, y) 
            && potentialFriendly.Controller == friendly;
    }

    public bool ValidStandardPlaySpace(int x, int y, Player friendly)
    {
        //first see if there's an adjacent friendly card to this space
        if (boardCtrl.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords(x, y, c, friendly))) return true;
        //if there isn't, check if the player is Surrounded
        else
        {
            //iterate through all possible spaces
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    //if there *is* a possible space to play it to, they're not surrounded
                    if (boardCtrl.ExistsCardOnBoard(c => IsFriendlyAdjacentToCoords(i, j, c, friendly))) return false;
                }
            }
            //if we didn't find a single place to play a card normally, any space is fair game, by the Surrounded rule
            return true;
        }
    }


    protected void ResetCardsForTurn()
    {
        foreach (var c in Cards) c?.ResetForTurn(TurnPlayer);
    }
}
