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
        //TODO deprecate change nesw

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