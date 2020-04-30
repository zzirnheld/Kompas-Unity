using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellCard : Card
{
    public const string SimpleSubtype = "Simple";
    public const string DelayedSubtype = "Delayed";

    public bool Fast { get; private set; }
    public int C { get; set; }
    public string Subtext { get; private set; }
    public string SpellSubtype { get; private set; }

    public override int Cost => C;
    public override string StatsString => Fast ? "Fast C" : "C" + C + " " + SpellSubtype;

    //get data
    public SerializableSpellCard GetSerializableVersion()
    {
        SerializableSpellCard serializableSpell = new SerializableSpellCard
        {
            cardName = CardName,
            effText = EffText,
            spellType = SpellSubtype,
            subtext = Subtext,
            c = C,

            location = location,
            owner = ControllerIndex,
            BoardX = boardX,
            BoardY = boardY,
            subtypeText = SubtypeText
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects)
    {
        if (!(serializedCard is SerializableSpellCard serializedSpell)) return;

        C = serializedSpell.c;
        Subtext = serializedSpell.subtext;
        SpellSubtype = serializedSpell.spellType;
        Fast = serializedSpell.fast;

        base.SetInfo(serializedCard, game, owner, effects);
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
        if (ControllerIndex == 0) transform.localEulerAngles = new Vector3(0, 0, 90);
        else transform.localEulerAngles = new Vector3(0, 0, 270);

    }
}
