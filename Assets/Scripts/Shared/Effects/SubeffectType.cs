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
    //TargetAugmentedCard = 7,
    ChooseFromList = 8,
    ChooseFromListSaveRest = 9,
    DeleteTargetFromList = 10,
    TargetAll = 11,

    //changing values, esp. on cards
    ChangeNESW = 100,
    AddPips = 101,
    PayPips = 102,
    XChangeNESW = 103,
    PayPipsByTargetCost = 150,

    //effect x
    SetXByBoardCount = 200,
    //SetXByTargetN = 250,
    //SetXByTargetE = 251,
    SetXByTargetS = 252,
    //SetXByTargetW = 253,
    //SetXByTargetCost = 254,

    //move cards between states
    PlayCard = 300,
    DiscardCard = 301,
    ReshuffleCard = 303,
    RehandCard = 304,
    Draw = 305,
    DrawX = 306,
    Bottomdeck = 307,
    Topdeck = 308,

    //loops/control flow
    XTimesLoop = 400,
    TTimesLoop = 401,
    WhileHaveTargetsLoop = 402,
    ExitLoopIfEffectImpossible = 403,
    JumpOnImpossible = 404,
    ClearOnImpossible = 405,
    ChooseEffectOption = 406,
    EndEffect = 407,
    CountXLoop = 408
}
