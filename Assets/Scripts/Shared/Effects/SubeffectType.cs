using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SubeffectType
{
    TargetCardOnBoard = 0,
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
    ChangeNESW = 100,
    AddPips = 101,
    PayPips = 102,
    //PlusMinusXN = 125,
    //PlusMinusXE = 126,
    //PlusMinusXS = 127,
    PlusMinusXW = 128,
    PayPipsByTargetCost = 150,
    SetXByBoardCount = 200,
    //SetXByTargetN = 250,
    //SetXByTargetE = 251,
    SetXByTargetS = 252,
    SetXByTargetW = 253,
    SetXByTargetCost = 254,
    PlayCard = 300,
    DiscardCard = 301,
    ReshuffleCard = 303,
    RehandCard = 304,
    XTimesLoop = 400,
    TTimesLoop = 401,
    WhileHaveTargetsLoop = 402,
    ExitLoopIfEffectImpossible = 403
}
