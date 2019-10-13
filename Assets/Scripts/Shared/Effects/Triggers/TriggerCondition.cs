using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TriggerCondition
{
    TurnStart = 0,
    Draw = 100,
    NESWChange = 200,
    Defends = 300,
    Attacks = 301,
    TakeCombatDamage = 302,
    DealCombatDamage = 303,
    Arrive = 400,
    Play = 401,
    Discard = 402
}
