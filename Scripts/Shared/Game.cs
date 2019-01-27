using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    
    public const bool DEBUG_MODE = true;

    public static Game mainGame;

    //other scripts
    public MouseController mouseCtrl;
    public NetworkController networkCtrl;
    public UIController uiCtrl;

    //game mechanics
    public BoardController boardCtrl;
    //game objects
    public GameObject boardObject;

    
    public int turnPlayer;
    protected Player[] players = new Player[2];
    public Player[] Players { get { return players; } }

    //game data
    List<Card> cards = new List<Card>();

    private void Update()
    {
        //tell the mouse controller to do stuff in the correct order
        //(this is in the game class because what it does will depend on what the target mode is later)
        //first, get the mouse ray (starts at the mouse, goes straight along direction camera points)
        mouseCtrl.GetMouseRay();
        //for now, assume that you're not targeting anything. here is the correct sequence of methods:
        //first drag anything that you were dragging last frame, even if your ray isn't on it now
        mouseCtrl.DragBeforeRaycast(); 
        //then, see if you've hit anything, whether or not you dragged anything
        mouseCtrl.GetRaycastHit();
        //then, check if you've clicked anything
        mouseCtrl.NormalClickObject();
    }

    //public static bool IsServerGame() { return mainGame is ServerGame; }
    //public static bool IsClientGame() { return mainGame is ClientGame; }

    public Card GetCardFromID(int id)
    {
        if (id > cards.Count) return null;

        return cards[id];
    }

    #region forwarding calls to correct controller
    //move cards between locations
    //TODO add is server checks to discard, topdeck, rehand
    public void Discard(Card card, int player = 0, bool ignoreClientServer = false)
    {
        Remove(card, player, ignoreClientServer);
        players[player].discardCtrl.AddToDiscard(card);
    }

    public void Discard(int cardID)
    {
        Discard(GetCardFromID(cardID));
    }

    public void Topdeck(Card card, int player = 0)
    {
        Remove(card, player);
        players[player].deckCtrl.PushTopdeck(card);
    }

    public void Topdeck(int cardID)
    {
        Topdeck(GetCardFromID(cardID));
    }

    public void Rehand(Card card, int player = 0)
    {
        Remove(card, player);
        players[player].handCtrl.AddToHand(card);
    }

    public void Rehand(int cardID)
    {
        Rehand(GetCardFromID(cardID));
    }

    public void Play(Card card, int toX, int toY, int player = 0, bool remove = true)
    {
        if(remove) Remove(card, player);

        boardCtrl.Play(card, toX, toY, player);
        ClientGame.mainClientGame.RequestPlay(card, toX, toY);
    }

    public void Play(int cardID, int toX, int toY)
    {
        Play(GetCardFromID(cardID), toX, toY);
    }

    public Card Draw(int player = 0)
    {
        Card toDraw = players[player].deckCtrl.PopTopdeck();
        players[player].handCtrl.AddToHand(toDraw);
        return toDraw;
    }

    /// <summary>
    /// Remove the card from wherever it is
    /// </summary>
    public virtual void Remove(Card toRemove, int player = 0, bool ignoreClientServer = false)
    {
        switch (toRemove.Location)
        {
        case Card.CardLocation.Field:
            boardCtrl.RemoveFromBoard(toRemove);
                break;
            case Card.CardLocation.Discard:
                players[player].discardCtrl.RemoveFromDiscard(toRemove);
                break;
            case Card.CardLocation.Hand:
                players[player].handCtrl.RemoveFromHand(toRemove);
                break;
            case Card.CardLocation.Deck:
                players[player].deckCtrl.RemoveFromDeck(toRemove);
                break;
            default:
                Debug.Log("Don't know how to remove " + toRemove.CardName);
                break;
            }
    }

    //moving
    public void Move(Card card, int toX, int toY)
    {
        boardCtrl.Move(card, toX, toY);
    }

    public void Move(int cardID, int toX, int toY)
    {
        boardCtrl.Move(GetCardFromID(cardID), toX, toY);
    }

    public void Swap(Card card, int toX, int toY)
    {
        boardCtrl.Swap(card, toX, toY);
    }

    //ui
    public virtual void SelectCard(Card card) { uiCtrl.SelectCard(card); }
    #endregion forwarding

    //game mechanics

    //requesting
    public virtual void RequestMove(Card card, int toX, int toY)
    {
        if (DEBUG_MODE) { Debug.Log("Debug Mode, not checking with server to move"); Move(card, toX, toY); }
        else throw new NotImplementedException();
    }
    public virtual void RequestPlay(Card card, int toX, int toY)
    {
        if (DEBUG_MODE) { Debug.Log("Debug Mode, not checking with server to play"); Play(card, toX, toY); }
        else throw new NotImplementedException();
    }


}
