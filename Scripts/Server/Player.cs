using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class Player {

    public ServerGame serverGame;

    //game mechanics data
    public int pips = 3;

    //other game data
    public bool friendly;
    public int index;

    //friendly
    public DeckController deckCtrl;
    public DiscardController discardCtrl;
    public HandController handCtrl;

    //friendly
    public GameObject deckObject;
    public GameObject discardObject;
    public GameObject handObject;

    private NetworkConnection connectionID;

    //getters and setters
    //game mechanics data
    //other game data 

    public NetworkConnection ConnectionID
    {
        get { return connectionID; }
    }

    public Player(NetworkConnection connectionID, int index, ServerGame serverGame)
    {
        this.connectionID = connectionID;
        this.index = index;
        this.serverGame = serverGame;
    }
}
