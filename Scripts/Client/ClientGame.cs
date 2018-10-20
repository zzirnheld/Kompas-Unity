using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGame : Game {


    private bool friendlyTurn; //TODO should it start with a value? should be init by server
    private int friendlyPips = 3;
    private int enemyPips = 3;

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
