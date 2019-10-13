using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class ClientGame : Game {

    public static ClientGame mainClientGame;

    private bool friendlyTurn;

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
    public ClientNotifier clientNotifier;
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
        players[0].enemy = players[1];
        //and the player2 stuff
        players[1].handCtrl = enemyHandCtrl;
        players[1].deckCtrl = enemyDeckCtrl;
        players[1].discardCtrl = enemyDiscardCtrl;
        players[1].handObject = enemyHandObj;
        players[1].deckObject = enemyDeckObj;
        players[1].discardObject = enemyDiscardObj;
        players[1].enemy = players[0];
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

    public void Delete(Card card)
    {
        Destroy(card.gameObject);
        //probably destroy and not set inactive because a card that is deleted and played again will just be created anew
        //card.gameObject.SetActive(false);
    }

    //requesting
    public void RequestMove(Card card, int toX, int toY)
    {
        clientNotifier.RequestMove(card, toX, toY);
    }

    public void RequestPlay(Card card, int toX, int toY)
    {
        clientNotifier.RequestPlay(card, toX, toY);
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
                clientNotifier.RequestTarget(card);

                //and change the game's target mode TODO should this do this
                targetMode = TargetMode.OnHold;
            }
        }
    }


}
