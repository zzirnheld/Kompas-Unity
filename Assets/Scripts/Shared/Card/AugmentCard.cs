using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AugmentCard : Card
{

    private Card augmentedCard;

    private int a;
    private string subtext;
    private string[] augSubtypes;
    private bool fast;

    public int A
    {
        get { return a; }
        set { a = value; }
    }

    public Card AugmentedCard
    {
        get { return augmentedCard; }
        set {
            augmentedCard = value;
            if(boardX != augmentedCard.BoardX || boardY != augmentedCard.BoardY)
                MoveTo(augmentedCard.BoardX, augmentedCard.BoardY);
        }
    }

    public string[] AugSubtypes
    {
        get => augSubtypes;
    }

    public override int Cost => A;
    public override string StatsString => fast ? "Fast A" : "A"  + A;

    //get data
    public SerializableAugCard GetSerializableVersion()
    {
        int index = augmentedCard.Augments.IndexOf(this);
        SerializableAugCard serializableSpell = new SerializableAugCard
        {
            cardName = CardName,
            effText = EffText,
            subtext = subtext,
            a = a,
            augSubtypes = augSubtypes,

            location = location,
            owner = ControllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = SubtypeText,
            index = index
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner)
    {
        if (!(serializedCard is SerializableAugCard serializedAug)) return;

        a = serializedAug.a;
        subtext = serializedAug.subtext;
        augSubtypes = serializedAug.augSubtypes;
        fast = serializedAug.fast;

        base.SetInfo(serializedCard, game, owner);
    }

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
        if (ControllerIndex == 0) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 0, 180);

    }

    public void Detach()
    {
        AugmentedCard = null;
    }
}
