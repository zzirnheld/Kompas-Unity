using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player {

    //game mechanics data
    int pips = 3;

    //other game data
    public bool friendly;
    private Quaternion cardRotation;

    //friendly
    public DeckController deckCtrl;
    public DiscardController discardCtrl;
    public HandController handCtrl;

    //friendly
    public GameObject deckObject;
    public GameObject discardObject;
    public GameObject handObject;
    
    private int connectionID;

    //getters and setters
    //game mechanics data
    //other game data 
    public Quaternion CardRotation { get { return cardRotation; } }

    public int ConnectionID
    {
        get { return connectionID; }
    }

    public Player(int connectionID)
    {
        this.connectionID = connectionID;
    }
}
