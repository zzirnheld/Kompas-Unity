using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Not abstract because it's instantiated as part of loading subeffects
/// </summary>
[System.Serializable]
public class Subeffect
{
    #region subeffect type constants
    //targeting
    public const string BoardTarget = "Board Target"; //0,
    public const string DeckTarget = "Deck Target"; //1,
    public const string DiscardTarget = "Discard Target"; //2,
    public const string HandTarget = "Hand Target"; //3,
    public const string SpaceTarget = "Space Target"; //4,
    public const string TargetThis = "Target This"; //5,
    public const string TargetThisSpace = "Target This Space"; //6,
    public const string TargetAugmentedCard = "Target Augmented Card"; //7,
    public const string ChooseFromList = "Choose From List"; //8,
    public const string ChooseFromListSaveRest = "Choose From List Saving Rest"; //9,
    public const string DeleteTargetFromList = "Delete Target"; //10,
    public const string TargetAll = "Target All"; //11,
    public const string AddRest = "Add Rest"; //12,
    public const string TargetDefender = "Target Defender"; //50,

    //changing values, esp. on cards
    //TODO deprecate change nesw
    public const string ChangeNESW = "Change NESW"; //100,
    public const string AddPips = "Add Pips"; //101,
    public const string PayPips = "Pay Pips"; //102,
    public const string XChangeNESW = "X Change NESW"; //103,
    public const string SwapNESW = "Swap NESW"; //104, //swaps two character's n, e, s, or w
    public const string Negate = "Negate"; //105,
    public const string Dispel = "Dispel"; //106,
    public const string SwapOwnNESW = "Swap Target's Own NESW"; //107, //swaps one character's n with e, etc.
    public const string ChangeSpellC = "Change C"; //108,
    public const string SetNESW = "Set NESW"; //109,
    public const string ChangeAllNESW = "Change NESW All"; //110,
    public const string SetAllNESW = "Set NESW All"; //111,
    public const string ResetStats = "Reset Stats"; //112,
    public const string Activate = "Activate"; //120,
    public const string SpendMovement = "Spend Movement"; //121,
    public const string TakeControl = "Take Control"; //130,
    //Deactivate = 121,
    public const string PayPipsByTargetCost = "Pay Target's Cost in Pips"; //150,

    //effect x
    public const string SetXByBoardCount = "Set X by Board Count"; //200,
    public const string SetXByGamestateValue = "Set X by Gamestate Value"; //201,
    public const string SetXByMath = "Set X by Math"; //202,
    public const string SetXByTargetValue = "Set X by Target Value"; //203,
    public const string ChangeXByGamestateValue = "Change X by Gamestate Value"; //211,
    //TODO deprecate next 2
    public const string SetXByTargetS = "Set X by Target S"; //252,
    public const string SetXByTargetCost = "Set X by Target Cost"; //254,
    public const string ChangeXByTargetValue = "Change X by Target Value"; //275,
    public const string PlayerChooseX = "Set X by Player Choice"; //299,

    //move cards between states
    public const string PlayCard = "Play"; //300,
    public const string DiscardCard = "Discard"; //301,
    public const string ReshuffleCard = "Reshuffle"; //303,
    public const string RehandCard = "Rehand"; //304,
    public const string Draw = "Draw"; //305,
    public const string DrawX = "Draw X"; //306,
    public const string Bottomdeck = "Bottomdeck"; //307,
    public const string Topdeck = "Topdeck"; //308,
    public const string Move = "Move"; //309,
    public const string Swap = "Swap"; //310,
    public const string Annihilate = "Annihilate"; //311,
    public const string BottomdeckRest = "Bottomdeck Rest"; //350,

    //loops/control flow
    public const string XTimesLoop = "Loop X Times"; //400,
    public const string TTimesLoop = "Loop T Times"; //401,
    public const string WhileHaveTargetsLoop = "Loop While Have Targets"; //402,
    public const string ExitLoopIfEffectImpossible = "Loop Until Effect Impossible"; //403,
    public const string JumpOnImpossible = "Jump on Effect Impossible"; //404,
    public const string ClearOnImpossible = "Clear Jump on Effect Impossible"; //405,
    public const string ChooseEffectOption = "Choose Effect Option"; //406,
    public const string EndEffect = "End Effect Resolution"; //407,
    public const string CountXLoop = "Count X Loop"; //408,
    public const string ConditionalEndEffect = "Conditionally End Effect Resolution"; //409,
    //TODO deprecate basic loop
    public const string BasicLoop = "Loop"; //410,
    public const string Jump = "Jump to Subeffect"; //411,

    //hanging effects
    public const string DelaySubeffect = "Delay Effect Resolution"; //500,
    public const string HangingNESWBuff = "Temporary NESW Buff"; //501,
    public const string HangingNESWBuffAll = "Temporary NESW Buff All"; //502,
    public const string HangingNegate = "Temporary Negate"; //503,
    public const string HangingActivate = "Temporary Activate"; //504,
    public const string HangingAnnihilate = "Hanging Annihilate"; //550,

    //misc
    public const string EndTurn = "End Turn"; //600,
    public const string Attack = "Attack"; //601
    #endregion subeffect type constants


    public virtual Effect Effect { get; }
    public virtual Player Controller { get; }
    public virtual Game Game { get; }

    public int SubeffIndex { get; protected set; }

    public GameCard Source => Effect.Source;

    /// <summary>
    /// The index in the Effect.targets array for which target this effect uses.
    /// If positive, just an index.
    /// If negative, it's Effect.targets.Count + targetIndex (aka that many back from the end)
    /// </summary>
    public int targetIndex = -1;

    /// <summary>
    /// The index in the coords list that this subeffect uses.
    /// If positive, just an index.
    /// If negative, it's Count + targetIndex (aka that many back)
    /// </summary>
    public int spaceIndex = -1;

    /// <summary>
    /// Represents the type of subeffect this is
    /// </summary>
    public string subeffType;

    public GameCard GetTarget(int num)
    {
        int trueIndex = num < 0 ? num + Effect.Targets.Count : num;
        return trueIndex < 0 ? null : Effect.Targets[trueIndex];
    }

    public (int x, int y) GetSpace(int num)
    {
        var trueIndex = num < 0 ? num + Effect.Coords.Count : num;
        return trueIndex < 0 ? (0, 0) : Effect.Coords[trueIndex];
    }

    public GameCard Target => GetTarget(targetIndex);
    public (int x, int y) Space => GetSpace(spaceIndex);
}
