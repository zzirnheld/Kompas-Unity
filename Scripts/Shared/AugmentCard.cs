using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentCard : Card {

    private CharacterCard thisCharacter;

    private int d; //TODO refactor this to A
    private string subtext;
    private bool fast;

    public int D
    {
        get { return d; }
        set { d = value; }
    }

    public CharacterCard ThisCharacter
    {
        get { return thisCharacter; }
        set {
            thisCharacter = value;
            MoveTo(thisCharacter.BoardX, thisCharacter.BoardY);
        }
    }

    //get data
    public SerializableAugCard GetSerializableVersion()
    {
        int index = thisCharacter.Augments.IndexOf(this);
        SerializableAugCard serializableSpell = new SerializableAugCard
        {
            cardName = cardName,
            effText = effText,
            subtext = subtext,
            d = d,

            location = location,
            owner = owner,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = subtypeText,
            index = index
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game)
    {
        if (!(serializedCard is SerializableAugCard)) return;
        SerializableAugCard serializedSpell = serializedCard as SerializableAugCard;

        d = serializedSpell.d;
        subtext = serializedSpell.subtext;
        fast = serializedSpell.fast;

        base.SetInfo(serializedCard, game);
    }
    
    public override int GetCost() { return D; }

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
        if (owner == 0) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 0, 180);

    }

    public void Detach()
    {
        ThisCharacter = null;
    }
}
