using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NetworkConnection = Unity.Networking.Transport.NetworkConnection;

public class Player {

    public ServerGame serverGame;
    public Player enemy;

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
    /// <summary>
    /// A list of potential fast cards and fast effects that the player has
    /// </summary>
    public List<Effect> responses;
    public List<Card> fastCards;

    /// <summary>
    /// Whether the player has yet passed priority
    /// </summary>
    public bool passedPriority = false;

    //other game data 
    /// <summary>
    /// The player can TODO click a checkbox to hold priority no matter what
    /// </summary>
    public bool holdPriority = false;
    /// <summary>
    /// The player TODO can uncheck a box to decline all fast effect activation opportunities
    /// </summary>
    public bool allowResponses = true;

    public NetworkConnection ConnectionID
    {
        get { return connectionID; }
    }

    public Player(NetworkConnection connectionID, int index, ServerGame serverGame)
    {
        this.connectionID = connectionID;
        this.index = index;
        this.serverGame = serverGame;
        responses = new List<Effect>();
        fastCards = new List<Card>();
    }

    public bool HoldsPriority()
    {
        return allowResponses && (holdPriority || responses.Count > 0 || fastCards.Count > 0);
    }
}
