using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{

    public enum SpellType { Simple, Enchant, Augment, Terraform, Delayed };

    private int d; //TODO refactor this to C
    private SpellType subtype;
    private string subtext;
    private bool fast;

    public int D
    {
        get { return d; }
        set { d = value; }
    }
    public string Subtext
    {
        get { return subtext; }
        set { subtext = value; }
    }
    public SpellType Subtype { get { return subtype; } }

    //get data
    public SerializableSpellCard GetSerializableVersion()
    {
        int index = -1;
        if (location == CardLocation.Hand) index = Game.mainGame.Players[owner].handCtrl.IndexOf(this);
        else if (location == CardLocation.Discard) index = Game.mainGame.Players[owner].discardCtrl.IndexOf(this);

        SerializableSpellCard serializableSpell = new SerializableSpellCard
        {
            cardName = cardName,
            effText = effText,
            subtype = subtype,
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
    public override void SetInfo(SerializableCard serializedCard)
    {
        if (!(serializedCard is SerializableSpellCard)) return;
        SerializableSpellCard serializedSpell = serializedCard as SerializableSpellCard;

        d = serializedSpell.d;
        subtext = serializedSpell.subtext;
        subtype = serializedSpell.subtype;
        fast = serializedSpell.fast;

        base.SetInfo(serializedCard);
    }


    public override int GetCost() { return D; }

    //game mechanics
    public override void MoveTo(int toX, int toY)
    {
        boardX = toX;
        boardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.03f);
        if (owner == 0) transform.localEulerAngles = new Vector3(0, 0, 90);
        else transform.localEulerAngles = new Vector3(0, 0, 270);

    }
}
