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

    //ui
    public override void SelectCard(Card card, bool fromClick)
    {
        uiCtrl.SelectCard(card, fromClick);
    }

    public void TargetCard(Card card)
    {
        //if the player is currently looking for a target on the board,
        if (targetMode == Game.TargetMode.BoardTarget)
        {
            //check if the target fits the restriction, according to us
            if (clientNetworkCtrl.lastRestriction.Evaluate(card, false))
            {
                //if it fits the restriction, send the proposed target to the server
                clientNetworkCtrl.RequestTarget(card);

                //and change the game's target mode
                targetMode = Game.TargetMode.NoTargeting;
            }
        }
    }


}
