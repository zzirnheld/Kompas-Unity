using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class ClientGame : Game {

    public static ClientGame mainClientGame;

    private bool friendlyTurn; //TODO should it start with a value? should be init by server
    //private int friendlyPips = 3;
    //private int enemyPips = 3;

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
    public ClientUIController clientUICtrl;

    private void Awake()
    {
        mainGame = this;
        mainClientGame = this;
        players[0] = new Player(default(NetworkConnection), 0, null);
        players[1] = new Player(default(NetworkConnection), 1, null);
        //set your stuff
        players[0].handCtrl = friendlyHandCtrl;
        players[0].deckCtrl = friendlyDeckCtrl;
        players[0].discardCtrl = friendlyDiscardCtrl;
        players[0].handObject = friendlyHandObj;
        players[0].deckObject = friendlyDeckObj;
        players[0].discardObject = friendlyDiscardObj;
        //and the player2 stuff
        players[1].handCtrl = enemyHandCtrl;
        players[1].deckCtrl = enemyDeckCtrl;
        players[1].discardCtrl = enemyDiscardCtrl;
        players[1].handObject = enemyHandObj;
        players[1].deckObject = enemyDeckObj;
        players[1].discardObject = enemyDiscardObj;
    }

    //game mechanics
    #region setting pips
    public void SetFriendlyPips(int num)
    {
        players[0].pips = num;
        uiCtrl.UpdateFriendlyPips(num);
    }

    public void SetEnemyPips(int num)
    {
        players[1].pips = num;
        uiCtrl.UpdateEnemyPips(num);
    }
    #endregion

    //requesting
    public void RequestMove(Card card, int toX, int toY)
    {
        clientNetworkCtrl.RequestMove(card, toX, toY);
    }

    public void RequestPlay(Card card, int toX, int toY)
    {
        clientNetworkCtrl.RequestPlay(card, toX, toY);
    }

    public void TargetCard(Card card)
    {
        //if the player is currently looking for a target on the board,
        if (targetMode == TargetMode.BoardTarget || targetMode == TargetMode.HandTarget)
        {
            //check if the target fits the restriction, according to us
            if (clientNetworkCtrl.lastRestriction.Evaluate(card))
            {
                //if it fits the restriction, send the proposed target to the server
                clientNetworkCtrl.RequestTarget(card);

                //and change the game's target mode TODO should this do this
                targetMode = TargetMode.OnHold;
            }
        }
    }


}
