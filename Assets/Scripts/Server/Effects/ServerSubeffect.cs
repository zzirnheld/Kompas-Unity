using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public abstract class ServerSubeffect : Subeffect
    {
        #region reasons for impossible
        public const string TargetWasNull = "No target to affect";
        public const string TargetAlreadyThere = "Target was already in the place to move it to";
        public const string NoValidCardTarget = "No valid card to target";
        public const string NoValidSpaceTarget = "No valid space to target";
        public const string ChangedStatsOfCardOffBoard = "Can't change stats of card not on the board";
        public const string EndOnPurpose = "Ended early on purpose";

        public const string CantAffordPips = "Can't afford pips";
        public const string CantAffordStats = "Can't afford stats";

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
        #endregion reasons for impossible

        public override Player Controller => EffectController;
        public override Effect Effect => ServerEffect;
        public override Game Game => ServerGame;

        public ServerEffect ServerEffect { get; protected set; }
        public ServerGame ServerGame => ServerEffect.serverGame;
        public ServerPlayer EffectController => ServerEffect.ServerController;
        public GameCard ThisCard => ServerEffect.Source;

        public ServerPlayer ServerPlayer => Player as ServerPlayer;

        /// <summary>
        /// Sets up the subeffect with whatever necessary values.
        /// Usually also initializes any restrictions the effects are using.
        /// </summary>
        /// <param name="eff">The effect this subeffect is part of.</param>
        /// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
        public virtual void Initialize(ServerEffect eff, int subeffIndex)
        {
            Debug.Log($"Finishing setup for new effect of type {GetType()}");
            ServerEffect = eff;
            SubeffIndex = subeffIndex;
        }

        /// <summary>
        /// Server Subeffect resolve method. Does whatever this type of subeffect does,
        /// then returns a ResolutionInfo struct containing what to do next.
        /// <returns><see langword="true"/> if the effect finished resolving successfully, <see langword="false"/> if it's awaiting response</returns>
        /// </summary>
        public abstract Task<ResolutionInfo> Resolve();


        /// <summary>
        /// Optional method. If implemented, does something when the effect is declared impossible.
        /// Default implementation just finishes resolution of the effect
        /// </summary>
        public virtual Task<ResolutionInfo> OnImpossible(string why)
        {
            Debug.Log($"Base On Impossible called for {GetType()}");
            ServerEffect.OnImpossible = null;
            return Task.FromResult(ResolutionInfo.Impossible(why));
        }
    }

    public struct ResolutionInfo
    {
        public const string EndedBecauseImpossible = "Ended because effect was impossible";

        public ResolutionResult result;

        public int index;

        public string reason;

        public static ResolutionInfo Next => new ResolutionInfo { result = ResolutionResult.Next };
        public static ResolutionInfo Index(int index) => new ResolutionInfo { result = ResolutionResult.Index, index = index };
        public static ResolutionInfo Impossible(string why) => new ResolutionInfo { result = ResolutionResult.Impossible, reason = why };
        public static ResolutionInfo End(string why) => new ResolutionInfo { result = ResolutionResult.End, reason = why };
    }

    public enum ResolutionResult
    {
        Next, Index, Impossible, End
    }
}