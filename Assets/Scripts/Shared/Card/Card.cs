using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

public abstract class Card : CardBase {
    public Game game;

    //stats
    public int Negations { get; private set; } = 0;
    public bool Negated {
        get => Negations > 0;
        set {
            if (value) Negations++;
            else Negations--;

            foreach (var e in Effects) e.Negated = value;
        }
    }
    public int Activations { get; private set; } = 0;
    public bool Activated
    {
        get => Activations > 0;
        set
        {
            if (value) Activations++;
            else Activations--;
        }
    }
    public abstract int Cost { get; }
    public abstract string StatsString { get; }
    public virtual bool Summoned { get => false; }

    //positioning
    public int BoardX { get; protected set; }
    public int BoardY { get; protected set; }
    public (int, int) Position { get => (BoardX, BoardY); }

    //movement
    public int SpacesMoved { get; protected set; }
    public virtual int SpacesCanMove { get => 0; }
    public MovementRestriction MovementRestriction { get; private set; }

    //attacking
    public AttackRestriction AttackRestriction { get; private set; }

    //playing
    public PlayRestriction PlayRestriction { get; private set; }

    //controller/owners
    public Player Controller { get; protected set; }
    public int ControllerIndex { get { return Controller.index; } }
    public Player Owner { get; private set; }
    public int OwnerIndex { get => Owner.index; }

    //misc
    public CardLocation Location { get; private set; }
    public int ID { get; private set; }
    public Effect[] Effects { get; private set; }
    public List<AugmentCard> Augments { get; private set; } = new List<AugmentCard>();
    
    public int IndexInList
    {
        get
        {
            switch (Location)
            {
                case CardLocation.Deck:
                    return Controller.deckCtrl.IndexOf(this);
                case CardLocation.Discard:
                    return Controller.discardCtrl.IndexOf(this);
                case CardLocation.Field:
                    return BoardX * 7 + BoardY;
                case CardLocation.Hand:
                    return Controller.handCtrl.IndexOf(this);
                default:
                    Debug.LogError($"Tried to ask for card index when in location {Location}");
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
    
    //image
    protected MeshRenderer meshRenderer;

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
        Debug.Log($"Attempting to move {CardName} from {this.Location} to {location}");

        //set the card's location variable
        this.Location = location;

        //set the parent to where we're going
        switch (location)
        {
            case CardLocation.Field:
                transform.SetParent(game.boardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Discard:
                transform.SetParent(Controller.discardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Hand:
                transform.SetParent(Controller.handObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Deck:
                transform.SetParent(Controller.deckObject.transform);
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
        SetImage(CardName);
    }

    public virtual void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects, int id)
    {
        base.SetInfo(serializedCard);

        this.Augments = new List<AugmentCard>();
        this.game = game;
        this.Owner = owner;
        this.ID = id;
        ChangeController(owner);

        this.Effects = effects;

        SpacesMoved = 0;
        MovementRestriction = serializedCard.MovementRestriction ?? new MovementRestriction();
        MovementRestriction.SetInfo(this);
        AttackRestriction = serializedCard.AttackRestriction ?? new AttackRestriction();
        AttackRestriction.SetInfo(this);
        PlayRestriction = serializedCard.PlayRestriction ?? new PlayRestriction();
        PlayRestriction.SetInfo(this);
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
    public virtual void ChangeController(Player newController)
    {
        Controller = newController;
        transform.localEulerAngles = new Vector3(0, 0, 180 * ControllerIndex);
    }

    /// <summary>
    /// Sets this card's x and y values and updates its transform
    /// </summary>
    public virtual void MoveTo(int toX, int toY, bool playerInitiated)
    {
        if(playerInitiated)
            SpacesMoved += System.Math.Abs(BoardX - toX) + System.Math.Abs(toY - toY);

        BoardX = toX;
        BoardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.1f);
        ChangeController(Controller);
        foreach (AugmentCard aug in Augments) aug.MoveTo(toX, toY, false);
    }

    public void PutBack()
    {
        Debug.Log($"Putting back {CardName} to {Location}");

        switch (Location)
        {
            case CardLocation.Deck:
                transform.SetParent(Controller.deckObject.transform);
                gameObject.SetActive(false);
                break;
            case CardLocation.Discard:
                transform.SetParent(Controller.discardObject.transform);
                transform.localPosition = new Vector3(0, 0, (float)Controller.discardCtrl.IndexOf(this) / -60f);
                break;
            case CardLocation.Field:
                transform.SetParent(game.boardObject.transform);
                transform.localPosition = new Vector3(GridIndexToPos(BoardX), GridIndexToPos(BoardY), -0.1f);
                break;
            case CardLocation.Hand:
                transform.SetParent(Controller.handObject.transform);
                Controller.handCtrl.SpreadOutCards();
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
    public virtual void ResetForTurn(Player turnPlayer)
    {
        foreach(Effect eff in Effects)
        {
            eff.ResetForTurn(turnPlayer);
        }

        SpacesMoved = 0;
    }

    #region augments
    public void AddAugment(AugmentCard augment)
    {
        if (augment == null) return;
        Augments.Add(augment);
        augment.AugmentedCard = this;
    }
    public bool HasAugment(AugmentCard augment) { return Augments.Contains(augment); }
    public void RemoveAugment(AugmentCard augment)
    {
        Augments.Remove(augment);
        augment.AugmentedCard = null;
    }
    public void RemoveAugmentAt(int index)
    {
        AugmentCard aug = Augments[index];
        Augments.RemoveAt(index);
        aug.AugmentedCard = null;
    }
    #endregion augments
}
