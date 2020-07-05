using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerCondition
{
    TurnStart = 0,
    EachDraw = 100,
    DrawX = 101,
    NESWChange = 200,
    Activate = 201,
    Deactivate = 202,
    NChange = 250,
    EChange = 251,
    SChange = 252,
    WChange = 253,
    CChange = 254,
    AChange = 255,
    Defends = 300,
    Attacks = 301,
    TakeCombatDamage = 302,
    DealCombatDamage = 303,
    Battles = 304,
    BattleEnds = 305,
    Arrive = 400,
    Play = 401,
    Discard = 402,
    Rehand = 403,
    Reshuffle = 404,
    Topdeck = 405,
    Bottomdeck = 406,
    ToDeck = 407,
    Move = 408,
    Annhilate = 409,
    Remove = 410,
    AugmentAttached = 450,
    AugmentDetached = 451
}
