using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trigger
{
    public const string TurnStart = "Turn Start"; //0,

    //change card stats
    public const string NESWChange = "NESW Change"; //200,
    public const string Activate = "Activate"; //201,
    public const string Deactivate = "Deactivate"; //202,
    public const string NChange = "N Change"; //250,
    public const string EChange = "E Change"; //251,
    public const string SChange = "S Change"; //252,
    public const string WChange = "W Change"; //253,
    public const string CChange = "C Change"; //254,
    public const string AChange = "A Change"; //255,

    //combat
    public const string Defends = "Defend"; //300,
    public const string Attacks = "Attack"; //301,
    public const string TakeCombatDamage = "Take Combat Damage"; //302,
    public const string DealCombatDamage = "Deal Combat Damage"; //303,
    public const string Battles = "Battle Start"; //304,
    public const string BattleEnds = "Battle End"; //305,

    //card moving
    public const string EachDraw = "Each Card Drawn"; //100,
    public const string DrawX = "Draw"; //101,
    public const string Arrive = "Arrive"; //400,
    public const string Play = "Play"; //401,
    public const string Discard = "Discard"; //402,
    public const string Rehand = "Rehand"; //403,
    public const string Reshuffle = "Reshuffle"; //404,
    public const string Topdeck = "Topdeck"; //405,
    public const string Bottomdeck = "Bottomdeck"; //406,
    public const string ToDeck = "To Deck"; //407,
    public const string Move = "Move"; //408,
    public const string Annhilate = "Annihilate"; //409,
    public const string Remove = "Remove"; //410,
    public const string AugmentAttached = "Augment Attached"; //450,
    public const string AugmentDetached = "Augment Detached"; //451

    public static readonly string[] TriggerConditions = {
        TurnStart,
        NESWChange, Activate, Deactivate, NChange, EChange, SChange, WChange, CChange, AChange,
        Defends, Attacks, TakeCombatDamage, DealCombatDamage, Battles, BattleEnds,
        EachDraw, DrawX, Arrive, Play, Discard, Rehand, Reshuffle, Topdeck, Bottomdeck, ToDeck, Move, Annhilate, Remove, AugmentAttached, AugmentDetached
    };

    public string triggerCondition;
    public TriggerRestriction triggerRestriction;

    public bool Optional = false;
    public string blurb = "";
}
