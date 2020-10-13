using KompasCore.Cards;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// Not abstract because it's instantiated as part of loading subeffects
    /// </summary>
    [System.Serializable]
    public class Subeffect
    {
        #region subeffect type constants
        //targeting
        public const string BoardTarget = "Board Target";
        public const string DeckTarget = "Deck Target";
        public const string DiscardTarget = "Discard Target";
        public const string HandTarget = "Hand Target";
        public const string SpaceTarget = "Space Target";
        public const string TargetThis = "Target This";
        public const string TargetThisSpace = "Target This Space";
        public const string TargetAugmentedCard = "Target Augmented Card";
        public const string TargetTargetsAugmentedCard = "Target Target's Augmented Card";
        public const string ChooseFromList = "Choose From List";
        public const string ChooseFromListSaveRest = "Choose From List Saving Rest";
        public const string DeleteTargetFromList = "Delete Target";
        public const string TargetAll = "Target All";
        public const string AddRest = "Add Rest";
        public const string TargetDefender = "Target Defender";
        public const string TargetAttacker = "Target Attacker";
        public const string TargetOtherInFight = "Target Other in Fight";
        public const string TargetAvatar = "Target Avatar";
        public const string TargetTriggeringCard = "Target Triggering Card";
        public const string TargetTriggeringCoords = "Target Triggering Space";
        public const string TargetTargetsSpace = "Target Target's Space";
        public const string AutoTargetSubeffect = "Auto Target";

        //changing values, esp. on cards
        //TODO deprecate change nesw
        public const string ChangeNESW = "Change NESW";
        public const string AddPips = "Add Pips";
        public const string PayPips = "Pay Pips";
        public const string XChangeNESW = "X Change NESW";
        public const string SwapNESW = "Swap NESW"; //swaps two characters' n, e, s, or w
        public const string Negate = "Negate";
        public const string Dispel = "Dispel";
        public const string SwapOwnNESW = "Swap Target's Own NESW"; //swaps one character's n with e, etc.
        public const string ChangeSpellC = "Change C";
        public const string SetNESW = "Set NESW";
        public const string ChangeAllNESW = "Change NESW All";
        public const string SetAllNESW = "Set NESW All";
        public const string ResetStats = "Reset Stats";
        public const string Activate = "Activate";
        public const string SpendMovement = "Spend Movement";
        public const string TakeControl = "Take Control";
        public const string PayPipsByTargetCost = "Pay Target's Cost in Pips";
        public const string Resummon = "Resummon";
        public const string ResummonAll = "Resummon All";

        public const string PayStats = "Pay Stats";
        public const string ChangeStats = "Change Stats";

        //effect x
        public const string SetXByBoardCount = "Set X by Board Count";
        public const string SetXByGamestateValue = "Set X by Gamestate Value";
        public const string SetXByMath = "Set X by Math";
        public const string SetXByTargetValue = "Set X by Target Value";
        public const string ChangeXByGamestateValue = "Change X by Gamestate Value";
                                                                                     //TODO deprecate next 2
        public const string SetXByTargetS = "Set X by Target S";
        public const string SetXByTargetCost = "Set X by Target Cost";
        public const string ChangeXByTargetValue = "Change X by Target Value";
        public const string PlayerChooseX = "Set X by Player Choice";

        //move cards between states
        public const string PlayCard = "Play";
        public const string DiscardCard = "Discard";
        public const string ReshuffleCard = "Reshuffle";
        public const string RehandCard = "Rehand";
        public const string Bottomdeck = "Bottomdeck";
        public const string Topdeck = "Topdeck";
        public const string Move = "Move";
        public const string Swap = "Swap";
        public const string Annihilate = "Annihilate";
        public const string Draw = "Draw";
        public const string DrawX = "Draw X";
        public const string Mill = "Mill";
        public const string BottomdeckRest = "Bottomdeck Rest";

        public const string AttachCard = "Attach";

        //loops/control flow
        public const string XTimesLoop = "Loop X Times";
        public const string TTimesLoop = "Loop T Times";
        public const string WhileHaveTargetsLoop = "Loop While Have Targets";
        public const string ExitLoopIfEffectImpossible = "Loop Until Effect Impossible";
        public const string JumpOnImpossible = "Jump on Effect Impossible";
        public const string ClearOnImpossible = "Clear Jump on Effect Impossible";
        public const string ChooseEffectOption = "Choose Effect Option";
        public const string EndEffect = "End Effect Resolution";
        public const string CountXLoop = "Count X Loop";
        public const string ConditionalEndEffect = "Conditionally End Effect Resolution";
                                                                                          //TODO deprecate basic loop
        public const string ConditionalJump = "Conditionally Jump";
        public const string BasicLoop = "Loop";
        public const string Jump = "Jump to Subeffect";

        //hanging effects
        public const string DelaySubeffect = "Delay Effect Resolution";
        public const string HangingNESWBuff = "Temporary NESW Buff";
        public const string HangingNESWBuffAll = "Temporary NESW Buff All";
        public const string HangingNegate = "Temporary Negate";
        public const string HangingActivate = "Temporary Activate";
        public const string HangingAnnihilate = "Hanging Annihilate";

        //misc
        public const string EndTurn = "End Turn";
        public const string Attack = "Attack";
        public const string ChangeLeyload = "Change Leyload";
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
        /// The index of player. If 0, this, if 1, enemy. 
        /// </summary>
        public int playerIndex = 0;

        /// <summary>
        /// Represents the type of subeffect this is
        /// </summary>
        public string subeffType;

        /// <summary>
        /// If the effect uses X, this is the multiplier to X
        /// </summary>
        public int xMultiplier = 0;

        /// <summary>
        /// If the effect uses X, this is the divisor to X
        /// </summary>
        public int xDivisor = 1;

        /// <summary>
        /// If the effect uses X, this is the modifier to X
        /// </summary>
        public int xModifier = 0;

        /// <summary>
        /// If the effect uses X, this is the adjusted value of X
        /// </summary>
        public int Count => (Effect.X * xMultiplier / xDivisor) + xModifier;

        public GameCard Target => Effect.GetTarget(targetIndex);
        public (int x, int y) Space => Effect.GetSpace(spaceIndex);
        public Player Player => Game.Players[(Controller.index + playerIndex) % Game.Players.Length];

        public bool RemoveTarget()
        {
            if (Target == null) return false;

            Effect.RemoveTarget(Target);
            return true;
        }
    }
}