using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGame : Game {

    //TODO override all play, move, etc. methods to call base and tell players to do the same
    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public static ServerGame mainServerGame;

    int currPlayerCount = 0; //current number of players. shouldn't exceed 2

    private void Awake()
    {
        mainGame = this;
        mainServerGame = this;
    }

    public bool AddPlayer(int connectionID)
    {
        if (currPlayerCount >= 2) return false;

        players[currPlayerCount] = new Player(connectionID);
        currPlayerCount++;

        return true;
    }

    public int GetPlayerIndexFromID(int connectionID)
    {
        if (currPlayerCount < 2) return -1;

        if (players[0].ConnectionID == connectionID) return 0;
        else return 1;
    }

    public Player GetPlayerFromID(int connectionID)
    {
        if (currPlayerCount < 2) return null;

        if (players[0].ConnectionID == connectionID) return players[0];
        else return players[1];
    }

    //do action given id
    #region playerIDActions
    public void SetPipsGivenPlayerID(int connectionID, int numPips)
    {
        GetPlayerFromID(connectionID).pips = numPips;
    }

    public Card RemoveFromHandGivenPlayerID(int connectionID, int index)
    {
        return GetPlayerFromID(connectionID).handCtrl.RemoveFromHandAt(index);
    }

    public Card RemoveFromDiscardGivenPlayerID(int connectionID, int index)
    {
        return GetPlayerFromID(connectionID).discardCtrl.CardAt(index, true); //true for remove
    }

    public Card RemoveFromDeckGivenPlayerID(int connectionID, string name)
    {
        return GetPlayerFromID(connectionID).deckCtrl.RemoveCardWithName(name);
    }

    public Card DrawGivenPlayerID(int connectionID)
    {
        return Draw(GetPlayerIndexFromID(connectionID));
    }

    public void AddToDiscardGivenPlayerID(int connectionID, Card card)
    {
        GetPlayerFromID(connectionID).discardCtrl.AddToDiscard(card);
    }
    #endregion

    //later, upgrade this with checking if the square is valid (adj or special case)
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

    //TODO: change turn

}
