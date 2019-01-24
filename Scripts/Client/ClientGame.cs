using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGame : Game {

    public static ClientGame mainClientGame;

    private bool friendlyTurn; //TODO should it start with a value? should be init by server
    private int friendlyPips = 3;
    private int enemyPips = 3;

    public HandController friendlyHandCtrl;
    public DeckController friendlyDeckCtrl;
    public DiscardController friendlyDiscardCtrl;

    public GameObject friendlyHandObj;
    public GameObject friendlyDeckObj;
    public GameObject friendlyDiscardObj;

    public HandController enemyHandCtrl;
    public DeckController enemyDeckCtrl;
    public DiscardController enemyDiscardCtrl;

    public GameObject enemyHandObj;
    public GameObject enemyDeckObj;
    public GameObject enemyDiscardObj;

    public ClientNetworkController clientNetworkCtrl;

    public int FriendlyPips
    {
        get { return friendlyPips; }
        set
        {
            friendlyPips = value;
            uiCtrl.friendlyPipsText.text = "Pips: " + friendlyPips;
        }
    }
    public int EnemyPips
    {
        get { return enemyPips; }
        set
        {
            enemyPips = value;
            uiCtrl.enemyPipsText.text = "Enemy Pips: " + enemyPips;
        }
    }

    private void Awake()
    {
        mainGame = this;
        mainClientGame = this;
        players[0] = new Player(0);
        players[1] = new Player(1);
        //set your stuff
        players[0].handCtrl = friendlyHandCtrl;
        players[0].deckCtrl = friendlyDeckCtrl;
        players[0].discardCtrl = friendlyDiscardCtrl;
        players[0].handObject = friendlyHandObj;
        players[0].deckObject = friendlyDeckObj;
        players[0].discardObject = friendlyDiscardObj;
        //and the enemy stuff
        players[1].handCtrl = enemyHandCtrl;
        players[1].deckCtrl = enemyDeckCtrl;
        players[1].discardCtrl = enemyDiscardCtrl;
        players[1].handObject = enemyHandObj;
        players[1].deckObject = enemyDeckObj;
        players[1].discardObject = enemyDiscardObj;
    }

    public override void Remove(Card toRemove, int player = 0, bool ignoreClientServer = false)
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
                //if it's our card, remove the exact card from our hand
                if (player == 0) friendlyHandCtrl.RemoveFromHand(toRemove);
                //if it's not, remove a random card (because it doesn't matter)
                else enemyHandCtrl.RemoveRandomCard(); //TODO remove the correct index
                break;
            case Card.CardLocation.Deck:
                if (player == 0) friendlyDeckCtrl.RemoveFromDeck(toRemove);
                else enemyDeckCtrl.PopTopdeck();
                break;
            default:
                Debug.Log("Unknown CardLocation to remove card from in ClientGame");
                break;
        }
    }

    //game mechanics
    //requesting
    public override void RequestMove(Card card, int toX, int toY)
    {
        clientNetworkCtrl.RequestMove(card, toX, toY);
    }

    public override void RequestPlay(Card card, int toX, int toY)
    {
            clientNetworkCtrl.RequestPlay(card, toX, toY);
    }

    //ui
    public override void SelectCard(Card card)
    {
        uiCtrl.SelectCard(card);
    }

}
