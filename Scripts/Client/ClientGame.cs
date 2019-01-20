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

    //game mechanics
    //requesting
    public override void RequestMove(Card card, int toX, int toY)
    {
        //TODO
        if (DEBUG_MODE)
        {
            Move(card, toX, toY);
            return;
        }
    }

    public override void RequestPlay(Card card, int toX, int toY)
    {
        //TODO
        if (DEBUG_MODE)
        {
            Play(card, toX, toY);
            return;
        }
    }

    //ui
    public override void SelectCard(Card card)
    {
        uiCtrl.SelectCard(card);
    }

}
