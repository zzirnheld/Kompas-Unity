using KompasCore.Cards;
using KompasCore.Exceptions;
using KompasCore.GameCore;

namespace KompasCore.Effects
{
    /// <summary>
    /// Not abstract because it's instantiated as part of loading subeffects
    /// </summary>
    [System.Serializable]
    public class Subeffect
    {
        #region reasons for impossible
        public const string TargetWasNull = "No target to affect";
        public const string TargetAlreadyThere = "Target was already in the place to move it to";
        public const string NoValidCardTarget = "No valid card to target";
        public const string NoValidSpaceTarget = "No valid space to target";
        public const string ChangedStatsOfCardOffBoard = "Can't change stats of card not on the board";
        public const string MovedCardOffBoard = "Moved card not on the board";
        public const string EndOnPurpose = "Ended early on purpose";

        public const string CantAffordPips = "Can't afford pips";
        public const string CantAffordStats = "Can't afford stats";

        public const string DeclinedFurtherTargets = "Declined further targets";

        //card movement failure
        public const string AnnihilationFailed = "Target couldn't be annihilated";
        public const string AttachFailed = "Attach as augment failed";
        public const string BottomdeckFailed = "Bottomdeck failed";
        public const string DiscardFailed = "Discard failed";
        public const string CouldntDrawAllX = "Couldn't draw all X cards";
        public const string CouldntMillAllX = "Couldn't mill all X cards";
        public const string MovementFailed = "Movement failed";
        public const string PlayingFailed = "Playing card failed";
        public const string RehandFailed = "Rehanding card failed";
        public const string ReshuffleFailed = "Reshuffle failed";
        public const string TopdeckFailed = "Topdeck failed";

        //misc
        public const string TooMuchEForHeal = "Target already has at least their printed E";
        #endregion reasons for impossible

        public virtual Effect Effect { get; }
        public virtual Player Controller { get; }
        public virtual Game Game { get; }

        public int SubeffIndex { get; protected set; }

        public GameCard Source => Effect.Source;
        public ActivationContext Context => Effect.CurrActivationContext;

        /// <summary>
        /// Represents the type of subeffect this is
        /// </summary>
        //public string subeffType;

        public bool forbidNotBoard = true;

        #region targeting indices
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
        public int playerIndex = -1;
        #endregion targeting indices

        #region effect x
        /// <summary>
        /// If the effect uses X, this is the multiplier to X. Default: 0
        /// </summary>
        public int xMultiplier = 0;

        /// <summary>
        /// If the effect uses X, this is the divisor to X. Default: 1
        /// </summary>
        public int xDivisor = 1;

        /// <summary>
        /// If the effect uses X, this is the modifier to X. Default: 0
        /// </summary>
        public int xModifier = 0;

        /// <summary>
        /// If the effect uses X, this is the adjusted value of X
        /// </summary>
        public int Count => (Effect.X * xMultiplier / xDivisor) + xModifier;
        #endregion effect x

        public GameCard Target => Effect.GetTarget(targetIndex);
        public Space Space => Effect.GetSpace(spaceIndex);
        public Player Player => Effect.GetPlayer(playerIndex);

        public void RemoveTarget()
        {
            if (Target == null) throw new NullCardException(TargetWasNull);

            Effect.RemoveTarget(Target);
        }
    }
}