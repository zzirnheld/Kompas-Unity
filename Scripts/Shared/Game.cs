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
    //friendly
    public DeckController friendlyDeckCtrl;
    public DiscardController friendlyDiscardCtrl;
    public HandController friendlyHandCtrl;
    //enemy
    public DeckController enemyDeckCtrl;
    public DiscardController enemyDiscardCtrl;
    public HandController enemyHandCtrl;

    //game objects
    public GameObject boardObject;
    //friendly
    public GameObject friendlyDeckObject;
    public GameObject friendlyDiscardObject;
    public GameObject friendlyHandObject;
    //enemy
    public GameObject enemyDeckObject;
    public GameObject enemyDiscardObject;
    public GameObject enemyHandObject;

    //game data


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

    #region forwarding calls to correct controller
    //move cards between locations
    public void Discard(Card card, bool friendly = true)
    {
        Remove(card, friendly);
        if (friendly) friendlyDiscardCtrl.AddToDiscard(card);
        else enemyDiscardCtrl.AddToDiscard(card);
    }
    public void Topdeck(Card card, bool friendly = true)
    {
        Remove(card, friendly);
        if (friendly) friendlyDeckCtrl.PushTopdeck(card);
        else enemyDeckCtrl.PushTopdeck(card);
    }
    public void Rehand(Card card, bool friendly = true)
    {
        Remove(card, friendly);
        if (friendly) friendlyHandCtrl.AddToHand(card);
        else enemyHandCtrl.AddToHand(card);
    }
    public void Play(Card card, int toX, int toY, bool friendly = true)
    {
        Remove(card, friendly);
        boardCtrl.Play(card, toX, toY, friendly);
    }

    public void Draw(bool friendly = true)
    {
        if (friendly) friendlyHandCtrl.AddToHand(friendlyDeckCtrl.PopTopdeck());
        else enemyHandCtrl.AddToHand(enemyDeckCtrl.PopTopdeck());
    }

    /// <summary>
    /// Remove the card from wherever it is
    /// </summary>
    public void Remove(Card toRemove, bool friendly = true)
    {
        switch (toRemove.Location)
        {
            case Card.CardLocation.Field:
                boardCtrl.RemoveFromBoard(toRemove);
                break;
            case Card.CardLocation.Discard:
                if (friendly) friendlyDiscardCtrl.RemoveFromDiscard(toRemove);
                else enemyDiscardCtrl.RemoveFromDiscard(toRemove);
                break;
            case Card.CardLocation.Hand:
                if (friendly) friendlyHandCtrl.RemoveFromHand(toRemove);
                else enemyHandCtrl.RemoveFromHand(toRemove);
                break;
            case Card.CardLocation.Deck:
                if (friendly) friendlyDeckCtrl.RemoveFromDeck(toRemove);
                else enemyDeckCtrl.RemoveFromDeck(toRemove);
                break;
        }
    }
    
    //moving
    public void Move(Card card, int toX, int toY)
    {
        boardCtrl.Move(card, toX, toY);
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
