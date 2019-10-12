using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Card : KompasObject {

    protected ClientGame clientGame;
    protected ServerGame serverGame;

    //constants
    //minimum and maximum distances to the board, discard, and deck objects for dragging
    public const float minBoardLocalX = -0.45f;
    public const float maxBoardLocalX = 0.45f;
    public const float minBoardLocalY = -0.45f;
    public const float maxBoardLocalY = 0.45f;
    public const float minDiscardX = 4.5f;
    public const float maxDiscardX = 5.5f;
    public const float minDiscardZ = -3.5f;
    public const float maxDiscardZ = -2.5f;
    public const float minDeckX = 2.5f;
    public const float maxDeckX = 3.5f;
    public const float minDeckZ = -5.5f;
    public const float maxDeckZ = -4.5f;

    //game mechanics data
    protected int boardX;
    protected int boardY;
    protected string cardName;
    protected string effText;
    protected string subtypeText;
    protected int controllerIndex;
    protected Player controller;
    protected int ownerIndex;
    protected Player owner;
    protected CardLocation location;
    protected int id;
    protected string[] subtypes;
    private List<AugmentCard> augments = new List<AugmentCard>();
    public List<AugmentCard> Augments { get { return augments; } }

    protected Effect[] effects;

    //other game data
    protected string cardFileName;
    protected MeshRenderer meshRenderer;
    protected Sprite detailedSprite;
    protected Sprite simpleSprite;
    protected bool dragging = false;

    //getters and setters
    //game mechanics data
    public int BoardX { get { return boardX; } }
    public int BoardY { get { return boardY; } }
    public string CardName
    {
        get { return cardName; }
        set { cardName = value; }
    }
    public string EffText
    {
        get { return effText; }
        set { effText = value; }
    }
    public string SubtypeText
    {
        get { return subtypeText; }
        set { subtypeText = value; }
    }
    public Player Controller { get { return controller; } }
    public int ControllerIndex { get { return controllerIndex; } }
    public Player Owner { get { return owner; } }
    public int OwnerIndex { get { return ownerIndex; } }
    public CardLocation Location { get { return location; } }
    public int ID
    {
        set
        {
            id = value;
        }
        get
        {
            return id;
        }
    }
    public string[] Subtypes { get { return subtypes; } }
    public Effect[] Effects { get => effects; }
    public abstract int Cost { get; }
    //other game data
    public string CardFileName {
        get { return cardFileName; }
        set { cardFileName = value; }
    }
    public Sprite DetailedSprite { get { return detailedSprite; } }
    public Sprite SimpleSprite { get { return simpleSprite; } }

    //unity methods
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }


    //set data
    /// <summary>
    /// Updates the location variable, sets active or not, and sets the parent transform.
    /// </summary>
    /// <param name="location">Where the card is going</param>
    public void SetLocation(CardLocation location)
    {
        Debug.Log($"Attempting to move {cardName} from {this.location} to {location}");

        //set the card's location variable
        this.location = location;

        //set the parent to where we're going
        switch (location)
        {
            case CardLocation.Field:
                transform.SetParent(game.boardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Discard:
                transform.SetParent(game.Players[controllerIndex].discardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Hand:
                transform.SetParent(game.Players[controllerIndex].handObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Deck:
                transform.SetParent(game.Players[controllerIndex].deckObject.transform);
                gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// Set the sprites of this card and gameobject
    /// </summary>
    public void SetImage(string cardFileName)
    {
        detailedSprite = Resources.Load<Sprite>("Detailed Sprites/" + cardFileName);
        simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
        //check if either is null. if so, log to debug and return
        if(detailedSprite == null || simpleSprite == null)
        {
            Debug.Log("Could not find sprite with name " + cardFileName);
            return;
        }
        //set this gameobject's texture to the simple sprite (by default, TODO change on zoom level change)
        Texture2D spriteTexture = simpleSprite.texture;
        //spriteTexture.alphaIsTransparency = true;
        meshRenderer.material.SetTexture("_MainTex", spriteTexture);
        //then make unity know it's a sprite so that it'll make the alpha transparent
        meshRenderer.material.shader = Shader.Find("Sprites/Default");
    }
    /// <summary>
    /// Set image by stored card file name
    /// </summary>
    public void SetImage()
    {
        SetImage(cardFileName);
    }

    public virtual void SetInfo(SerializableCard serializedCard, Game game, int ownerIndex)
    {
        this.game = game;
        clientGame = game as ClientGame;
        serverGame = game as ServerGame;

        cardName = serializedCard.cardName;
        effText = serializedCard.effText;
        subtypeText = serializedCard.subtypeText;
        location = serializedCard.location;
        subtypes = serializedCard.subtypes;

        //could also be      serializedCard.effects == null ? serializedCard.effects.Length : 0
        effects = new Effect[serializedCard.effects?.Length ?? 0];
        this.owner = game.Players[ownerIndex];
        this.ownerIndex = ownerIndex;
        ChangeController(ownerIndex);

        //go through each of the serialized effects, 
        for (int i = 0; i < (serializedCard.effects?.Length ?? 0); i++)
        {
            effects[i] = new Effect(serializedCard.effects[i], this, controllerIndex);
        }

        this.augments = new List<AugmentCard>();
        if (location == CardLocation.Field) MoveTo(serializedCard.BoardX, serializedCard.BoardY);
        else
        {
            boardX = serializedCard.BoardX;
            boardY = serializedCard.BoardY;
        }
    }

    //game mechanics
    #region distance/adjacency
    public int DistanceTo(int x, int y)
    {
        return Mathf.Abs(x - BoardX) > Mathf.Abs(y - BoardY) ? Mathf.Abs(x - BoardX) : Mathf.Abs(y - BoardY);
        /* equivalent to
         * if (Mathf.Abs(card.X - X) > Mathf.Abs(card.Y - Y)) return Mathf.Abs(card.X - X);
         * else return Mathf.Abs(card.Y - Y);
         * is card.X - X > card.Y - Y? If so, return card.X -X, otherwise return card.Y - Y
        */
    }
    public int DistanceTo(Card card)
    {
        return DistanceTo(card.BoardX, card.BoardY);
    }
    public bool WithinSlots(int numSlots, Card card)
    {
        if (card == null) return false;
        return DistanceTo(card) <= numSlots;
    }
    public bool WithinSlots(int numSlots, int x, int y)
    {
        return DistanceTo(x, y) <= numSlots;
    }
    public bool IsAdjacentTo(Card card)
    {
        return DistanceTo(card) == 1;
    }
    public bool IsAdjacentTo(int x, int y)
    {
        return DistanceTo(x, y) == 1;
    }
    #endregion distance/adjacency

    //misc mechanics methods
    public void ChangeController(int owner)
    {
        this.controllerIndex = owner;
        this.controller = game.Players[controllerIndex];
        transform.localEulerAngles = new Vector3(0, 0, 180 * owner);
    }

    /// <summary>
    /// Sets this card's x and y values and updates its transform
    /// </summary>
    public virtual void MoveTo(int toX, int toY)
    {
        boardX = toX;
        boardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.1f);
        ChangeController(controllerIndex);
        foreach (AugmentCard aug in augments) aug.MoveTo(toX, toY);
    }

    public void PutBack()
    {
        if (location == CardLocation.Deck)
            gameObject.SetActive(false);
        else if (location == CardLocation.Discard)
            transform.localPosition = new Vector3(0, 0, (float) game.Players[controllerIndex].discardCtrl.IndexOf(this) / -60f);
        else if (location == CardLocation.Field)
            transform.localPosition = new Vector3(GridIndexToPos(boardX), GridIndexToPos(boardY), -0.1f);
        else if (location == CardLocation.Hand)
            game.Players[controllerIndex].handCtrl.SpreadOutCards();
    }

    /// <summary>
    /// Resets any of the card's values that might be different from their originals.
    /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
    /// </summary>
    public virtual void ResetCard() { }

    //helper methods
    public bool WithinIgnoreY(Vector3 position, float minX, float maxX, float minZ, float maxZ)
    {
        return position.x > minX && position.x < maxX && position.z > minZ && position.z < maxZ;
    }
    public bool WithinIgnoreZ(Vector3 position, float minX, float maxX, float minY, float maxY)
    {
        return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
    }

    /// <summary>
    /// Resets anything that needs to be reset for the start of the turn.
    /// </summary>
    public virtual void ResetForTurn()
    {
        foreach(Effect e in effects)
        {
            e.ResetForTurn();
        }
    }

    #region move card between areas
    private void Remove()
    {
        switch (location)
        {
            case CardLocation.Field:
                game.boardCtrl.RemoveFromBoard(this);
                break;
            case CardLocation.Discard:
                controller.discardCtrl.RemoveFromDiscard(this);
                break;
            case CardLocation.Hand:
                controller.handCtrl.RemoveFromHand(this);
                break;
            case CardLocation.Deck:
                controller.deckCtrl.RemoveFromDeck(this);
                break;
            default:
                Debug.Log("Tried to remove card from " + location);
                break;
        }
    }

    public void Discard()
    {
        Remove();
        controller.discardCtrl.AddToDiscard(this);
    }

    public void Rehand(int controllerIndex)
    {
        Remove();
        ChangeController(controllerIndex);
        controller.handCtrl.AddToHand(this);
    }

    public void Rehand()
    {
        Rehand(controllerIndex);
    }

    public void Reshuffle()
    {
        Remove();
        controller.deckCtrl.ShuffleIn(this);
    }

    public void Topdeck()
    {
        Remove();
        controller.deckCtrl.PushTopdeck(this);
    }

    public void Play(int toX, int toY, int controllerIndex)
    {
        Remove();
        game.boardCtrl.Play(this, toX, toY, controllerIndex);
    }

    public void Play(int toX, int toY)
    {
        Play(toX, toY, controllerIndex);
    }

    public void MoveOnBoard(int toX, int toY)
    {
        game.boardCtrl.Move(this, toX, toY);
    }
    #endregion move card between areas

    #region augments
    public void AddAugment(AugmentCard augment)
    {
        if (augment == null) return;
        augments.Add(augment);
        augment.ThisCard = this;
    }
    public bool HasAugment(AugmentCard augment) { return augments.Contains(augment); }
    public void RemoveAugment(AugmentCard augment)
    {
        augments.Remove(augment);
        augment.ThisCard = null;
    }
    public void RemoveAugmentAt(int index)
    {
        AugmentCard aug = augments[index];
        augments.RemoveAt(index);
        aug.ThisCard = null;
    }
    #endregion augments

    #region MouseStuff
    //actual interaction
    public override void OnClick()
    {
        game.uiCtrl.SelectCard(this, true);
    }
    public override void OnHover()
    {
        game.uiCtrl.HoverOver(this);
    }
    /// <summary>
    /// The mouse position contains x and z values of the absolute position of where the ray intersects whatever it hits, and y = 2.
    /// </summary>
    public override void OnDrag(Vector3 mousePos)
    {
        if (game.targetMode != Game.TargetMode.Free) return;

        if (!dragging)
        {
            dragging = true;
            transform.parent = game.boardObject.transform;
        }

        transform.position = mousePos;
    }
    public override void OnDragEnd(Vector3 mousePos)
    {
        if (game.targetMode != Game.TargetMode.Free) return;
        dragging = false; //dragging has ended

        //to be able to use local coordinates to see if you're on the board, set parent to game board
        transform.parent = game.boardObject.transform;

        //then, check if it's on the board, accodring to the local coordinates of the game board)
        if (WithinIgnoreZ(transform.localPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
        {
            //if the card is being moved on the field, that means it's just being moved
            if (location == CardLocation.Field)
            {
                int x = PosToGridIndex(transform.localPosition.x);
                int y = PosToGridIndex(transform.localPosition.y);
                //then check if it's an attack or not
                if (game.boardCtrl.GetCharAt(x, y) != null && game.boardCtrl.GetCharAt(x, y).ControllerIndex != controllerIndex)
                    clientGame.clientNetworkCtrl.RequestAttack(this, x, y);
                else
                    clientGame.clientNetworkCtrl.RequestMove(this, x, y);
            }
            //otherwise, it is being played from somewhere like the hand or discard
            else
                clientGame.clientNetworkCtrl.RequestPlay(this,
                    PosToGridIndex(transform.localPosition.x), PosToGridIndex(transform.localPosition.y));
        }
        //if it's not on the board, maybe it's on top of the discard
        else if (WithinIgnoreY(transform.position, minDiscardX, maxDiscardX, minDiscardZ, maxDiscardZ))
        {
            //in that case, discard it //TODO do this by raycasting along another layer to see if you hit deck/discard
            clientGame.clientNetworkCtrl.RequestDiscard(this);
        }
        //maybe it's on top of the deck
        else if (WithinIgnoreY(transform.position, minDeckX, maxDeckX, minDeckZ, maxDeckZ))
        {
            //in that case, topdeck it
            clientGame.clientNetworkCtrl.RequestTopdeck(this);
        }
        //if it's not in any of those, probably should go back in the hand.
        else
        {
            clientGame.clientNetworkCtrl.RequestRehand(this);
        }
    }
    #endregion MouseStuff
}
