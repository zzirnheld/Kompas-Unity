using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentCard : Card {

    private Card thisCard;

    private int a; //TODO refactor this to A
    private string subtext;
    private string[] augSubtypes;
    private bool fast;

    public int A
    {
        get { return a; }
        set { a = value; }
    }

    public Card ThisCard
    {
        get { return thisCard; }
        set {
            thisCard = value;
            MoveTo(thisCard.BoardX, thisCard.BoardY);
        }
    }

    public string[] AugSubtypes
    {
        get => augSubtypes;
    }

    //get data
    public SerializableAugCard GetSerializableVersion()
    {
        int index = thisCard.Augments.IndexOf(this);
        SerializableAugCard serializableSpell = new SerializableAugCard
        {
            cardName = cardName,
            effText = effText,
            subtext = subtext,
            d = a,
            augSubtypes = augSubtypes,

            location = location,
            owner = controllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = subtypeText,
            index = index
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game, int ownerIndex)
    {
        if (!(serializedCard is SerializableAugCard)) return;
        SerializableAugCard serializedAug = serializedCard as SerializableAugCard;

        a = serializedAug.d;
        subtext = serializedAug.subtext;
        augSubtypes = serializedAug.augSubtypes;
        fast = serializedAug.fast;

        base.SetInfo(serializedCard, game, ownerIndex);
    }
    
    public override int GetCost() { return A; }

    //game mechanics
    //TODO prevent z fighting
    public override void MoveTo(int toX, int toY)
    {
        boardX = toX;
        boardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.05f);
        if (controllerIndex == 0) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 0, 180);

    }

    public void Detach()
    {
        ThisCard = null;
    }
}
