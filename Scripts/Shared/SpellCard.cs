using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellCard : Card
{

    public enum SpellType { Simple, Enchant, Augment, Terraform, Delayed };

    private int c;
    private SpellType spellSubtype;
    private string subtext;
    private bool fast;

    public int C
    {
        get { return c; }
        set { c = value; }
    }
    public string Subtext
    {
        get { return subtext; }
        set { subtext = value; }
    }
    public SpellType SpellSubtype { get { return spellSubtype; } }

    public override int Cost { get { return C; } }

    //get data
    public SerializableSpellCard GetSerializableVersion()
    {
        int index = -1;
        if (location == CardLocation.Hand) index = game.Players[controllerIndex].handCtrl.IndexOf(this);
        else if (location == CardLocation.Discard) index = game.Players[controllerIndex].discardCtrl.IndexOf(this);

        SerializableSpellCard serializableSpell = new SerializableSpellCard
        {
            cardName = cardName,
            effText = effText,
            subtype = spellSubtype,
            subtext = subtext,
            d = c,

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
        if (!(serializedCard is SerializableSpellCard serializedSpell)) return;

        c = serializedSpell.d;
        subtext = serializedSpell.subtext;
        spellSubtype = serializedSpell.subtype;
        fast = serializedSpell.fast;

        base.SetInfo(serializedCard, game, ownerIndex);
    }

    //game mechanics
    public override void MoveTo(int toX, int toY)
    {
        boardX = toX;
        boardY = toY;

        /* for setting where the gameobject is, it would be x and z, except that the quad is turned 90 degrees
         * so we change the local x and y. the z coordinate also therefore needs to be negative
         * to show the card above the game board on the screen. */
        transform.localPosition = new Vector3(GridIndexToPos(toX), GridIndexToPos(toY), -0.03f);
        if (controllerIndex == 0) transform.localEulerAngles = new Vector3(0, 0, 90);
        else transform.localEulerAngles = new Vector3(0, 0, 270);

    }
}
