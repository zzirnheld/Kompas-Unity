using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class Card : KompasObject {

    public enum CardLocation { Nowhere, Field, Discard, Hand, Deck };

    private ClientGame clientGame;

    //constants
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
    public const float spacesInGrid = 7;
    public const float boardLenOffset = 0.45f;

    //game mechanics data
    protected int boardX;
    protected int boardY;
    protected string cardName;
    protected string effText;
    protected string subtypeText;
    protected int owner; //TODO differentiate betw owner & ctrler
    protected CardLocation location;
    protected int id;
    protected string[] subtypes;

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
    public int Owner { get { return owner; } }
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
        Debug.Log("Attempting to move " + cardName + " from " + this.location + " to " + location);

        //if we're leaving somewhere, do something about it
        if (location != this.location)
        {
            switch (this.location)
            {
                case CardLocation.Field:
                    game.boardCtrl.RemoveFromBoard(this);
                    break;
                case CardLocation.Discard:
                    game.Players[owner].discardCtrl.RemoveFromDiscard(this);
                    break;
                case CardLocation.Hand:
                    game.Players[owner].handCtrl.RemoveFromHand(this);
                    break;
                case CardLocation.Deck:
                    game.Players[owner].deckCtrl.RemoveFromDeck(this);
                    break;
            }
        }

        //set the location
        this.location = location;

        //set the parent to where we're going
        switch (location)
        {
            case CardLocation.Field:
                transform.SetParent(game.boardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Discard:
                transform.SetParent(game.Players[owner].discardObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Hand:
                transform.SetParent(game.Players[owner].handObject.transform);
                gameObject.SetActive(true);
                break;
            case CardLocation.Deck:
                transform.SetParent(game.Players[owner].deckObject.transform);
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

    public virtual void SetInfo(SerializableCard serializedCard, Game game)
    {
        this.game = game;
        clientGame = game as ClientGame;

        cardName = serializedCard.cardName;
        effText = serializedCard.effText;
        subtypeText = serializedCard.subtypeText;
        location = serializedCard.location;

        //go through each of the serialized effects, 

        ChangeController(serializedCard.owner);
        if (location == CardLocation.Field) MoveTo(serializedCard.BoardX, serializedCard.BoardY);
        else
        {
            boardX = serializedCard.BoardX;
            boardY = serializedCard.BoardY;
        }
    }

    //game mechanics
    //helper methods
    protected int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Divide by 0.9f because the range of accepted position values is 0 to 0.9f (0.45 - -0.45).
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)((pos + boardLenOffset) * spacesInGrid / 0.9f);
    }
    protected float GridIndexToPos(int gridIndex)
    {
        /* first, cast the index to a float to make sure the math works out.
         * then, divide by the grid length to board ratio to get a number (0,1) that makes
         * sense in the context of the board's local lenth of one.
         * then, subtract the board length offset to get a number that makes sense
         * in the actual board's context of values (-0.45, 0.45) (legal local coordinates)
         * finally, add 0.025 to account for the 0.05 space on either side of the legal 0.45 area
         */
        return (((float)gridIndex) / spacesInGrid - boardLenOffset + 0.025f);
    }

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
        if (Location != CardLocation.Field || card.Location != CardLocation.Field) return 0;
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

    /*public int GetIndexInList()
    {
        if (location == CardLocation.Discard) return Game.mainGame.Players[owner].discardCtrl.IndexOf(this);
        else if (location == CardLocation.Hand) return Game.mainGame.Players[owner].handCtrl.IndexOf(this);
        else if (location == CardLocation.Deck && Game.IsServerGame()) return Game.mainGame.Players[owner].deckCtrl.IndexOf(this);
        else return -1;
    }*/

    //card moving methods
    public void Discard()
    {
        game.Discard(this);
    }
    public void Topdeck()
    {
        game.Topdeck(this);
    }
    public void Rehand()
    {
        game.Rehand(this);
    }

    //misc mechanics methods
    public void ChangeController(int owner)
    {
        this.owner = owner;
        if (owner == 0) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 0, 180);
        //TODO anything else?
    }
    public virtual int GetCost() { return 0; }

    //playing cards
    /// <summary>
    /// sets this card's x and y values and updates where its gameobject is.
    /// does NOT set M if the card is a character
    /// </summary>
    public virtual void MoveTo(int toX, int toY)
    {
        boardX = toX;
        boardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.1f);
        ChangeController(owner);
        //if (owner == 0) transform.localEulerAngles = Vector3.zero;
        //else transform.localEulerAngles = new Vector3(0, 0, 180);
    }

    public void PutBack()
    {
        if (location == CardLocation.Deck)
            gameObject.SetActive(false);
        else if (location == CardLocation.Discard)
            transform.localPosition = new Vector3(0, 0, (float) game.Players[owner].discardCtrl.IndexOf(this) / -60f);
        else if (location == CardLocation.Field)
            transform.localPosition = new Vector3(GridIndexToPos(boardX), GridIndexToPos(boardY), -0.1f);
        else if (location == CardLocation.Hand)
            game.Players[owner].handCtrl.SpreadOutCards();
    }

    //interaction methods
    //helper methods
    public bool WithinIgnoreY(Vector3 position, float minX, float maxX, float minZ, float maxZ)
    {
        return position.x > minX && position.x < maxX && position.z > minZ && position.z < maxZ;
    }
    public bool WithinIgnoreZ(Vector3 position, float minX, float maxX, float minY, float maxY)
    {
        return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
    }

    //actual interaction
    public override void OnClick()
    {
        game.SelectCard(this);
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
        if (!dragging)
        {
            dragging = true;
            transform.parent = game.boardObject.transform;
        }

        transform.position = mousePos;
    }
    public override void OnDragEnd(Vector3 mousePos)
    {
        Debug.Log("Drag ended at absolute position: " + transform.position + ", local position: " + transform.localPosition);
        dragging = false; //dragging has ended

        //to be able to use local coordinates to see if you're on the board, set parent to game board
        transform.parent = game.boardObject.transform;

        //then, check if it's on the board, accodring to the local coordinates of the game board)
        if (WithinIgnoreZ(transform.localPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
        {
            Debug.Log(cardName + " ended drag on the field");
            //if the card is being moved on the field, that means it's just being moved
            if (location == CardLocation.Field)
            {
                clientGame.clientNetworkCtrl.RequestMove(this,
                    PosToGridIndex(transform.localPosition.x), PosToGridIndex(transform.localPosition.y));
            }
            //otherwise, it is being played from somewhere like the hand or discard
            else
                clientGame.clientNetworkCtrl.RequestPlay(this,
                    PosToGridIndex(transform.localPosition.x), PosToGridIndex(transform.localPosition.y));
        }
        //if it's not on the board, maybe it's on top of the discard
        else if (WithinIgnoreY(transform.position, minDiscardX, maxDiscardX, minDiscardZ, maxDiscardZ))
        {
            Debug.Log(cardName + " ended drag on discard");
            //in that case, discard it //TODO do this by raycasting along another layer to see if you hit deck/discard
            clientGame.clientNetworkCtrl.RequestDiscard(this);
        }
        //maybe it's on top of the deck
        else if (WithinIgnoreY(transform.position, minDeckX, maxDeckX, minDeckZ, maxDeckZ))
        {
            Debug.Log(cardName + " ended drag on the deck");
            //in that case, topdeck it
            clientGame.clientNetworkCtrl.RequestTopdeck(this);
        }
        //if it's not in any of those, probably should go back in the hand.
        else
        {
            Debug.Log(cardName + " ended drag nowhere. putting it in the hand. was at " + transform.position);
            clientGame.clientNetworkCtrl.RequestRehand(this);
        }
    }

}
