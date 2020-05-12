using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SubeffectType
{
    //targeting
    BoardTarget = 0,
    DeckTarget = 1,
    DiscardTarget = 2,
    HandTarget = 3,
    SpaceTarget = 4,
    TargetThis = 5,
    TargetThisSpace = 6,
    TargetAugmentedCard = 7,
    ChooseFromList = 8,
    ChooseFromListSaveRest = 9,
    DeleteTargetFromList = 10,
    TargetAll = 11,

    //changing values, esp. on cards
    ChangeNESW = 100,
    AddPips = 101,
    PayPips = 102,
    XChangeNESW = 103,
    SwapNESW = 104, //swaps two character's n, e, s, or w
    Negate = 105,
    Dispel = 106,
    SwapOwnNESW = 107, //swaps one character's n with e, etc.
    ChangeSpellC = 108,
    SetNESW = 109,
    ChangeAllNESW = 110,
    Activate = 120,
    //Deactivate = 121,
    PayPipsByTargetCost = 150,

    //effect x
    SetXByBoardCount = 200,
    SetXByGamestateValue = 201,
    SetXByMath = 202,
    ChangeXByGamestateValue = 211,
    //SetXByTargetN = 250,
    //SetXByTargetE = 251,
    SetXByTargetS = 252,
    //SetXByTargetW = 253,
    SetXByTargetCost = 254,
    PlayerChooseX = 299,

    //move cards between states
    PlayCard = 300,
    DiscardCard = 301,
    ReshuffleCard = 303,
    RehandCard = 304,
    Draw = 305,
    DrawX = 306,
    Bottomdeck = 307,
    Topdeck = 308,
    Move = 309,

    //loops/control flow
    XTimesLoop = 400,
    TTimesLoop = 401,
    WhileHaveTargetsLoop = 402,
    ExitLoopIfEffectImpossible = 403,
    JumpOnImpossible = 404,
    ClearOnImpossible = 405,
    ChooseEffectOption = 406,
    EndEffect = 407,
    CountXLoop = 408,
    ConditionalEndEffect = 409,

    //hanging effects
    DelaySubeffect = 500,
    HangingNESWBuff = 501,
    HangingNESWBuffAll = 502,
    HangingNegate = 503
}
