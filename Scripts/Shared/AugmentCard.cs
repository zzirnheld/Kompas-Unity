using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AugmentCard : Card {

    private CharacterCard thisCharacter;

    public CharacterCard ThisCharacter
    {
        get { return thisCharacter; }
        set { thisCharacter = value; }
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
        if (friendly) transform.localEulerAngles = Vector3.zero;
        else transform.localEulerAngles = new Vector3(0, 0, 180);

    }

    public void Detach()
    {
        ThisCharacter = null;
    }
}
