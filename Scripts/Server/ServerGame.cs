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

    private void Awake()
    {
        mainGame = this;
    }

}
