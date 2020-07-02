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
}
