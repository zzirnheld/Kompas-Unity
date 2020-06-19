using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpellCard : Card
{
    public const string SimpleSubtype = "Simple";
    public const string DelayedSubtype = "Delayed";
    public const string TerraformSubtype = "Terraform";
    public const string VanishingSubtype = "Vanishing";

    public bool Fast { get; private set; }
    public int C { get; set; }
    public string Subtext { get; private set; }
    public string SpellSubtype { get; private set; }
    public int Arg { get; private set; }

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
            subtypeText = SubtypeText
        };
        return serializableSpell;
    }

    //set data
    public override void SetInfo(SerializableCard serializedCard, Game game, Player owner, Effect[] effects, int id)
    {
        if (!(serializedCard is SerializableSpellCard serializedSpell)) return;

        C = serializedSpell.c;
        Subtext = serializedSpell.subtext;
        SpellSubtype = serializedSpell.spellType;
        Fast = serializedSpell.fast;
        Arg = serializedSpell.arg;

        base.SetInfo(serializedCard, game, owner, effects, id);
    }

    //game mechanics
    public override void MoveTo(int toX, int toY, bool playerInitiated)
    {
        base.MoveTo(toX, toY, playerInitiated);

        transform.localPosition = BoardController.GridIndicesFromPos(toX, toY);
    }

    public override void ChangeController(Player newController)
    {
        base.ChangeController(newController);
        transform.localEulerAngles = new Vector3(0, 0, 90 + 180 * ControllerIndex);
    }

    public override bool CardInAOE(Card c) => SpellSubtype == TerraformSubtype && DistanceTo(c) <= Arg;

    public override bool SpaceInAOE(int x, int y) => SpellSubtype == TerraformSubtype && DistanceTo(x, y) <= Arg;

    public override void ResetForTurn(Player turnPlayer)
    {
        base.ResetForTurn(turnPlayer);
        if (SpellSubtype == VanishingSubtype && TurnsOnBoard >= Arg) game.Discard(this);
    }
}
