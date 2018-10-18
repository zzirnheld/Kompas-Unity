using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {
    
    public const bool DEBUG_MODE = true;

    public static Game mainGame;

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

    //ui


    //game mechanics
    //move cards between locations
    public void Discard(Card card)
    {
        //TODO
    }
    public void Topdeck(Card card)
    {
        //TODO
    }
    public void Rehand(Card card)
    {
        //TODO
    }
    public void Play(Card card, int toX, int toY)
    {
        //TODO
    }

    //moving
    public void Move(Card card, int toX, int toY)
    {
        //TODO
    }
    
    //requesting
    public virtual void RequestMove(Card card, int toX, int toY) { }
    public virtual void RequestPlay(Card card, int toX, int toY) { }
    
    //ui
    public virtual void SelectCard(Card card) { }

}
