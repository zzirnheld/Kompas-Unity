using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public abstract class Card : CardBase {
    public Game game;

    public ClientGame clientGame { get; protected set; }
    public ServerGame serverGame { get; protected set; }

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
    protected bool dragging = false;

    public const float spacesInGrid = 7f;
    public const float boardLenOffset = 0.45f;

    protected static int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Divide by 0.9f because the range of accepted position values is 0 to 0.9f (0.45 - -0.45).
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)(((pos + boardLenOffset) * (spacesInGrid - 1f) / (2 * boardLenOffset)) + 0.5f);
    }
    protected static float GridIndexToPos(int gridIndex)
    {
        /* first, cast the index to a float to make sure the math works out.
         * then, divide by the grid length to board ratio to get a number (0,1) that makes
         * sense in the context of the board's local lenth of one.
         * then, subtract the board length offset to get a number that makes sense
         * in the actual board's context of values (-0.45, 0.45) (legal local coordinates)
         * finally, add 0.025 to account for the 0.05 space on either side of the legal 0.45 area
         */
        return (((float)(gridIndex)) / (spacesInGrid - 1f) * (2 * boardLenOffset)) - boardLenOffset;
    }

    //getters and setters
    //game mechanics data
    public int BoardX { get { return boardX; } }
    public int BoardY { get { return boardY; } }
    public Player Controller { get { return controller; } }
    public int ControllerIndex { get { return Controller.index; } }
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
                transform.SetParent(controller.discardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Hand:
                transform.SetParent(controller.handObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Deck:
                transform.SetParent(controller.deckObject.transform);
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

    public virtual void SetInfo(SerializableCard serializedCard, Game game, Player owner)
    {
        base.SetInfo(serializedCard);
        location = serializedCard.location;

        this.augments = new List<AugmentCard>();

        this.game = game;
        clientGame = game as ClientGame;
        serverGame = game as ServerGame;

        this.owner = owner;
        this.ownerIndex = owner.index;
        ChangeController(owner);

        //could also be      serializedCard.effects == null ? serializedCard.effects.Length : 0
        effects = new Effect[serializedCard.effects?.Length ?? 0];

        //go through each of the serialized effects, creating it
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new Effect(serializedCard.effects[i], this, ControllerIndex);
        }
        if (serverGame != null)
        {
            foreach (Effect eff in effects)
            {
                if (eff.Trigger != null)
                {
                    Debug.Log("registering trigger for " + eff.Trigger.triggerCondition);
                    serverGame.RegisterTrigger(eff.Trigger.triggerCondition, eff.Trigger);
                }
                else Debug.Log("trigger is null");
            }
        }

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
    public void ChangeController(Player newController)
    {
        controller = newController;
        transform.localEulerAngles = new Vector3(0, 0, 180 * ControllerIndex);
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
        ChangeController(controller);
        foreach (AugmentCard aug in augments) aug.MoveTo(toX, toY);
    }

    public void PutBack()
    {
        if (location == CardLocation.Deck)
            gameObject.SetActive(false);
        else if (location == CardLocation.Discard)
            transform.localPosition = new Vector3(0, 0, (float) controller.discardCtrl.IndexOf(this) / -60f);
        else if (location == CardLocation.Field)
            transform.localPosition = new Vector3(GridIndexToPos(boardX), GridIndexToPos(boardY), -0.1f);
        else if (location == CardLocation.Hand)
            controller.handCtrl.SpreadOutCards();
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
        foreach(Effect eff in effects)
        {
            eff.ResetForTurn();
        }
    }

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
    private void GoToMouse()
    {
        //raycast to get point to drag to
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
        }
    }

    //actual interaction
    public void OnMouseDrag()
    {
        if (dragging == false)
        {
            dragging = true;
            transform.parent = game.boardObject.transform;
        }

        GoToMouse();
    }

    public void OnMouseExit()
    {
        bool mouseDown = Input.GetMouseButton(0);
        if (dragging)
        {
            if (mouseDown) GoToMouse();
            else OnMouseUp();
        }

        dragging = mouseDown;
    }

    public void OnMouseUp()
    {
        Debug.Log("On end drag");
        if (!dragging) return;
        dragging = false;

        if (game.targetMode != Game.TargetMode.Free) return;

        //to be able to use local coordinates to see if you're on the board, set parent to game board
        transform.parent = game.boardObject.transform;

        //then, check if it's on the board, accodring to the local coordinates of the game board)
        if (WithinIgnoreZ(transform.localPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
        {
            //if the card is being moved on the field, that means it's just being moved
            if (location == CardLocation.Field)
            {
                Debug.Log($"Local position: {transform.localPosition.x}, {transform.localPosition.y}");
                int x = PosToGridIndex(transform.localPosition.x);
                int y = PosToGridIndex(transform.localPosition.y);
                CharacterCard charThere = game.boardCtrl.GetCharAt(x, y);
                Debug.Log($"Trying to move/attack to {x}, {y}, the controller index, if any, is {charThere?.ControllerIndex}");
                //then check if it's an attack or not
                if (charThere != null && charThere.ControllerIndex != ControllerIndex)
                    clientGame.clientNotifier.RequestAttack(this, x, y);
                else
                    clientGame.clientNotifier.RequestMove(this, x, y);
            }
            //otherwise, it is being played from somewhere like the hand or discard
            else
                clientGame.clientNotifier.RequestPlay(this,
                    PosToGridIndex(transform.localPosition.x), PosToGridIndex(transform.localPosition.y));
        }
        //if it's not on the board, maybe it's on top of the discard
        else if (WithinIgnoreY(transform.position, minDiscardX, maxDiscardX, minDiscardZ, maxDiscardZ))
        {
            //in that case, discard it //TODO do this by raycasting along another layer to see if you hit deck/discard
            clientGame.clientNotifier.RequestDiscard(this);
        }
        //maybe it's on top of the deck
        else if (WithinIgnoreY(transform.position, minDeckX, maxDeckX, minDeckZ, maxDeckZ))
        {
            //in that case, topdeck it
            clientGame.clientNotifier.RequestTopdeck(this);
        }
        //if it's not in any of those, probably should go back in the hand.
        else
        {
            clientGame.clientNotifier.RequestRehand(this);
        }
    }

    public void OnMouseEnter()
    {
        Debug.Log("On mouse enter");
        game.uiCtrl.HoverOver(this);
    }

    public void OnMouseDown()
    {
        Debug.Log("on mouse down");
        game.uiCtrl.SelectCard(this, true);
    }
    #endregion MouseStuff
}
