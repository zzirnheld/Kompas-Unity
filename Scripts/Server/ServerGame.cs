using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class ServerGame : Game {

    //TODO override all play, move, etc. methods to call base and tell players to do the same
    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public static ServerGame mainServerGame;

    int currPlayerCount = 0; //current number of players. shouldn't exceed 2
    public int cardCount = 0;

    public HandController player1HandCtrl;
    public DeckController player1DeckCtrl;
    public DiscardController player1DiscardCtrl;

    public GameObject player1HandObj;
    public GameObject player1DeckObj;
    public GameObject player1DiscardObj;

    public HandController player2HandCtrl;
    public DeckController player2DeckCtrl;
    public DiscardController player2DiscardCtrl;

    public GameObject player2HandObj;
    public GameObject player2DeckObj;
    public GameObject player2DiscardObj;

    private void Awake()
    {
        mainGame = this;
        mainServerGame = this;
        /*
        players[0] = new Player(0);
        players[1] = new Player(1);
        //set your stuff
        players[0].handCtrl = player1HandCtrl;
        players[0].deckCtrl = player1DeckCtrl;
        players[0].discardCtrl = player1DiscardCtrl;
        players[0].handObject = player1HandObj;
        players[0].deckObject = player1DeckObj;
        players[0].discardObject = player1DiscardObj;
        //and the player2 stuff
        players[1].handCtrl = player2HandCtrl;
        players[1].deckCtrl = player2DeckCtrl;
        players[1].discardCtrl = player2DiscardCtrl;
        players[1].handObject = player2HandObj;
        players[1].deckObject = player2DeckObj;
        players[1].discardObject = player2DiscardObj;*/
    }

    public int AddPlayer(NetworkConnection connectionID)
    {
        if (currPlayerCount >= 2) return -1;

        players[currPlayerCount] = new Player(connectionID, currPlayerCount);
        if(currPlayerCount == 0)
        {
            players[0].handCtrl = player1HandCtrl;
            players[0].deckCtrl = player1DeckCtrl;
            players[0].discardCtrl = player1DiscardCtrl;
            players[0].handObject = player1HandObj;
            players[0].deckObject = player1DeckObj;
            players[0].discardObject = player1DiscardObj;
        }
        else if(currPlayerCount == 1)
        {
            players[1].handCtrl = player2HandCtrl;
            players[1].deckCtrl = player2DeckCtrl;
            players[1].discardCtrl = player2DiscardCtrl;
            players[1].handObject = player2HandObj;
            players[1].deckObject = player2DeckObj;
            players[1].discardObject = player2DiscardObj;
        }
        currPlayerCount++;
        return currPlayerCount;
    }

    public int GetPlayerIndexFromID(NetworkConnection connectionID)
    {
        for(int i = 0; i < currPlayerCount; i++)
        {
            if (players[i].ConnectionID == connectionID) return i;
        }

        return -1;
    }

    public Player GetPlayerFromID(NetworkConnection connectionID)
    {
        for (int i = 0; i < currPlayerCount; i++)
        {
            if (players[i].ConnectionID == connectionID) return players[i];
        }

        return null;
    }

    public bool HasPlayer2()
    {
        return currPlayerCount >= 2;
    }

    public void GiveTurnPlayerPips()
    {
        (networkCtrl as ServerNetworkController).SetPips(turnPlayer, players[turnPlayer].ConnectionID, players[turnPlayer].pips + MaxCardsOnField);
    }

    //do action given id
    #region playerIDActions
    public void SetPipsGivenPlayerID(NetworkConnection connectionID, int numPips)
    {
        GetPlayerFromID(connectionID).pips = numPips;
    }

    public Card RemoveFromHandGivenPlayerID(NetworkConnection connectionID, int index)
    {
        return GetPlayerFromID(connectionID).handCtrl.RemoveFromHandAt(index);
    }

    public Card RemoveFromDiscardGivenPlayerID(NetworkConnection connectionID, int index)
    {
        return GetPlayerFromID(connectionID).discardCtrl.CardAt(index, true); //true for remove
    }

    public Card RemoveFromDeckGivenPlayerID(NetworkConnection connectionID, string name)
    {
        return GetPlayerFromID(connectionID).deckCtrl.RemoveCardWithName(name);
    }

    public Card DrawGivenPlayerID(NetworkConnection connectionID)
    {
        return Draw(GetPlayerIndexFromID(connectionID));
    }

    public void AddToDiscardGivenPlayerID(NetworkConnection connectionID, Card card)
    {
        GetPlayerFromID(connectionID).discardCtrl.AddToDiscard(card);
    }
    #endregion

    //later, upgrade this with checking if the square is valid (adj or special case)
    #region check validity
    public bool ValidBoardPlay(Card card, int toX, int toY)
    {
        return card != null 
            && (card is CharacterCard || card is SpellCard)
            && boardCtrl.ValidIndices(toX, toY) 
            && boardCtrl.GetCardAt(toX, toY) == null;
    }

    public bool ValidAugment(Card card, int toX, int toY)
    {
        return card != null
            && card is AugmentCard
            && boardCtrl.ValidIndices(toX, toY)
            && boardCtrl.GetCharAt(toX, toY) != null;
    }

    public bool ValidMove(Card toMove, int toX, int toY)
    {
        //when make automated, add logic to determine if it's a valid move
        //for now, tho, just allow
        return true;

        /*(Card fromCard = boardCtrl.GetCardAt(fromX, fromY);
        Card toCard = boardCtrl.GetCardAt(toX, toY);
        return toCard == null ||
            (fromCard is CharacterCard && toCard is CharacterCard);*/
    }

    #endregion
    
    public void SwitchTurn()
    {
        turnPlayer = 1 - turnPlayer;
        GiveTurnPlayerPips();

        //draw for turn and store what was drawn
        (networkCtrl as ServerNetworkController).AttemptToDraw(turnPlayer, players[turnPlayer].ConnectionID);
    }
}
