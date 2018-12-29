using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerGame : Game {

    //TODO override all play, move, etc. methods to call base and tell players to do the same
    //model is basically: players request to the server to do something:
    //if server oks, it tells all players to do the thing
    //if server doesn't ok, it sends to all players a "hold up reset everything to how it should be"

    public int turnPlayer;

    private Player[] players = new Player[2];
    int currPlayerCount = 0; //current number of players. shouldn't exceed 2

    public Player[] Players { get { return players; } }

    private void Awake()
    {
        mainGame = this;
    }

    public bool AddPlayer(int connectionID)
    {
        if (currPlayerCount >= 2) return false;

        players[currPlayerCount] = new Player(connectionID);
        currPlayerCount++;

        return true;
    }

    //later, upgrade this with checking if the square is valid (adj or special case)
    public bool ValidSummon(CharacterCard character, int toX, int toY)
    {
        return character != null && boardCtrl.ValidIndices(toX, toY);
    }
    //TODO: change turn

}
