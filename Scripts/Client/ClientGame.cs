using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientGame : Game {

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
        //TODO
    }

}
