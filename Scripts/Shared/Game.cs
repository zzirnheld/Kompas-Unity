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
    public DeckController deckCtrl;
    public DiscardController discardCtrl;
    public HandController handCtrl;

    //game objects
    public GameObject boardObject;
    public GameObject deckObject;
    public GameObject discardObject;
    public GameObject handObject;

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
    public void Discard(Card card)
    {
        Remove(card);
        discardCtrl.AddToDiscard(card);
    }
    public void Topdeck(Card card)
    {
        Remove(card);
        deckCtrl.PushTopdeck(card);
    }
    public void Rehand(Card card)
    {
        Remove(card);
        handCtrl.AddToHand(card);
    }
    public void Play(Card card, int toX, int toY)
    {
        Remove(card);
        boardCtrl.Play(card, toX, toY);
    }

    public void Draw()
    {
        handCtrl.AddToHand(deckCtrl.PopTopdeck());
    }

    /// <summary>
    /// Remove the card from wherever it is
    /// </summary>
    public void Remove(Card toRemove)
    {
        switch (toRemove.Location)
        {
            case Card.CardLocation.Field:
                boardCtrl.RemoveFromBoard(toRemove);
                break;
            case Card.CardLocation.Discard:
                discardCtrl.RemoveFromDiscard(toRemove);
                break;
            case Card.CardLocation.Hand:
                handCtrl.RemoveFromHand(toRemove);
                break;
            case Card.CardLocation.Deck:
                deckCtrl.RemoveFromDeck(toRemove);
                break;
        }
    }
    
    //moving
    public void Move(Card card, int toX, int toY)
    {
        //TODO
        boardCtrl.Move(card, toX, toY);
    }
    public void Swap(Card card, int toX, int toY)
    {
        //TODO
        boardCtrl.Swap(card, toX, toY);
    }

    //ui
    public virtual void SelectCard(Card card) { uiCtrl.SelectCard(card); }
    #endregion forwarding

    //game mechanics

    //requesting
    public virtual void RequestMove(Card card, int toX, int toY) { }
    public virtual void RequestPlay(Card card, int toX, int toY) { }


}
