using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using KompasNetworking;

public abstract class Player : MonoBehaviour{

    public abstract Player Enemy { get; }

    //game mechanics data
    public int pips = 3;
    public AvatarCard Avatar;

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

    public TcpClient TcpClient { get; private set; }

    //game mechanics data
    /// <summary>
    /// A list of potential fast cards and fast effects that the player has
    /// </summary>
    public List<Effect> responses;
    public List<GameCard> fastCards;

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

    private void Awake()
    {
        responses = new List<Effect>();
        fastCards = new List<GameCard>();
    }

    public virtual void SetInfo(TcpClient tcpClient, int index)
    {
        TcpClient = tcpClient;
        this.index = index;
    }

    public bool HoldsPriority()
    {
        return allowResponses && (holdPriority || responses.Count > 0 || fastCards.Count > 0);
    }
}
