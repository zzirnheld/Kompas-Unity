using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Card : KompasObject {

    public enum CardLocation { Field, Discard, Hand, Deck };

    //constants
    public const float minBoardLocalX = -0.45f;
    public const float maxBoardLocalX = 0.45f;
    public const float minBoardLocalZ = -0.45f;
    public const float maxBoardLocalZ = 0.45f;
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
    protected Player controller;
    protected CardLocation location;

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
    public Player Controller { get { return controller; } }
    public CardLocation Location { get { return location; } }
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

    //set game mechanics data
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
        meshRenderer.material.SetTexture("_MainTex", simpleSprite.texture);
    }
    /// <summary>
    /// Set image by stored card file name
    /// </summary>
    public void SetImage()
    {
        SetImage(cardFileName);
    }

    //game mechanics
    //helper methods
    protected int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)((pos + boardLenOffset) * spacesInGrid + 0.5f);
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

    //misc mechanics methods
    public void ChangeController(Player newController)
    {
        controller = newController;
        transform.localRotation = newController.CardRotation;
        //TODO anything else?
    }

    //playing cards

    //interaction methods
    //helper methods
    public bool Within(Vector3 position, float minX, float maxX, float minZ, float maxZ)
    {
        return position.x > minX && position.x < maxX && position.z > minZ && position.z < maxZ;
    }

    //actual interaction
    public override void OnClick()
    {
        Game.mainGame.SelectCard(this);
    }
    public override void OnHover()
    {
        //TODO show enlarged detailed sprite
    }
    /// <summary>
    /// The mouse position contains x and z values of the absolute position of where the ray intersects whatever it hits, and y = 2.
    /// </summary>
    public override void OnDrag(Vector3 mousePos)
    {
        if (!dragging)
        {
            dragging = true;
            transform.parent = Game.mainGame.fieldObject.transform;
        }
        
        transform.position = mousePos;
    }


}
