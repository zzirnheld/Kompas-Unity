using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public abstract class Card : CardBase {
    public Game game;

    //game mechanics data
    protected int boardX;
    protected int boardY;
    protected Player controller;
    protected int ownerIndex;
    protected Player owner;
    protected CardLocation location;
    protected int id;
    protected string[] subtypes;
    private List<AugmentCard> augments = new List<AugmentCard>();
    public List<AugmentCard> Augments { get { return augments; } }
    public bool Negated { get; protected set; }

    protected Effect[] effects;

    //other game data
    protected string cardFileName;
    protected MeshRenderer meshRenderer;

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

    public abstract string StatsString { get; }

    public int IndexInList
    {
        get
        {
            switch (location)
            {
                case CardLocation.Deck:
                    return controller.deckCtrl.IndexOf(this);
                case CardLocation.Discard:
                    return controller.discardCtrl.IndexOf(this);
                case CardLocation.Field:
                    return boardX * 7 + boardY;
                case CardLocation.Hand:
                    return controller.handCtrl.IndexOf(this);
                default:
                    Debug.LogError($"Tried to ask for card index when in location {location}");
                    return -1;
            }
        }
    }


    public const float spacesInGrid = 7f;
    public const float boardLenOffset = 0.45f;
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
        Debug.Log($"Attempting to move {CardName} from {this.location} to {location}");

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

        foreach (Effect eff in effects)
        {
            if (eff.Trigger != null)
            {
                Debug.Log($"Registering triggered effect of {CardName} to {eff.Trigger.triggerCondition}");
                game.RegisterTrigger(eff.Trigger.triggerCondition, eff.Trigger);
            }
            else Debug.Log($"Registering activated effect of {CardName}");
        }

        if (location == CardLocation.Field) MoveTo(serializedCard.BoardX, serializedCard.BoardY);
        else
        {
            boardX = serializedCard.BoardX;
            boardY = serializedCard.BoardY;
        }
    }
    
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
        Debug.Log($"Putting back {CardName} to {location}");

        switch (location)
        {
            case CardLocation.Deck:
                transform.SetParent(controller.deckObject.transform);
                gameObject.SetActive(false);
                break;
            case CardLocation.Discard:
                transform.SetParent(controller.discardObject.transform);
                transform.localPosition = new Vector3(0, 0, (float)controller.discardCtrl.IndexOf(this) / -60f);
                break;
            case CardLocation.Field:
                transform.SetParent(game.boardObject.transform);
                transform.localPosition = new Vector3(GridIndexToPos(boardX), GridIndexToPos(boardY), -0.1f);
                break;
            case CardLocation.Hand:
                transform.SetParent(controller.handObject.transform);
                controller.handCtrl.SpreadOutCards();
                break;
        }
    }

    /// <summary>
    /// Resets any of the card's values that might be different from their originals.
    /// Should be called when cards move out the discard, or into the hand, deck, or annihilation
    /// </summary>
    public virtual void ResetCard() { }

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

    public virtual void Negate()
    {
        Negated = true;
        foreach(var e in effects)  e.Negate();
    }

    #region augments
    public void AddAugment(AugmentCard augment)
    {
        if (augment == null) return;
        augments.Add(augment);
        augment.AugmentedCard = this;
    }
    public bool HasAugment(AugmentCard augment) { return augments.Contains(augment); }
    public void RemoveAugment(AugmentCard augment)
    {
        augments.Remove(augment);
        augment.AugmentedCard = null;
    }
    public void RemoveAugmentAt(int index)
    {
        AugmentCard aug = augments[index];
        augments.RemoveAt(index);
        aug.AugmentedCard = null;
    }
    #endregion augments
}
