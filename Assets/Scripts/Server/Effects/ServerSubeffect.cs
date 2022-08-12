using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasServer.Effects
{
    public abstract class ServerSubeffect : Subeffect
    {
        public override Player Controller => EffectController;
        public override Effect Effect => ServerEffect;
        public override Game Game => ServerGame;

        public ServerEffect ServerEffect { get; protected set; }
        public ServerGame ServerGame => ServerEffect.serverGame;
        public ServerPlayer EffectController => ServerEffect.ServerController;
        public GameCard ThisCard => ServerEffect.Source;

        public ServerPlayer ServerPlayer => PlayerTarget as ServerPlayer;

        public EffectInitializationContext DefaultInitializationContext
            => Effect.CreateInitializationContext(this, default);

        /// <summary>
        /// Sets up the subeffect with whatever necessary values.
        /// Usually also initializes any restrictions the effects are using.
        /// </summary>
        /// <param name="eff">The effect this subeffect is part of.</param>
        /// <param name="subeffIndex">The index in the subeffect array of its parent <paramref name="eff"/> this subeffect is.</param>
        public virtual void Initialize(ServerEffect eff, int subeffIndex)
        {
            //Debug.Log($"Finishing setup for new subeffect of type {GetType()}");
            ServerEffect = eff;
            SubeffIndex = subeffIndex;
        }

        /// <summary>
        /// Server Subeffect resolve method. Does whatever this type of subeffect does
        /// <returns>A ResolutionInfo object describing what to do next</returns>
        /// </summary>
        public abstract Task<ResolutionInfo> Resolve();

        /// <summary>
        /// Whether this subeffect will be considered EffectImpossible at this point
        /// </summary>
        /// <returns></returns>
        public virtual bool IsImpossible() => true;


        /// <summary>
        /// Optional method. If implemented, does something when the effect is declared impossible.
        /// Default implementation just finishes resolution of the effect
        /// </summary>
        public virtual Task<ResolutionInfo> OnImpossible(string why)
        {
            ServerEffect.OnImpossible = null;
            return Task.FromResult(ResolutionInfo.Impossible(why));
        }

        protected void CreateCardLink(params GameCard[] cards)
        {
            GameCard[] validCards = cards.Where(c => c != null).ToArray();
            //if (validCards.Length <= 1) return; //Don't create a link between one non-null card? nah, do, so we can delete it as expected later

            var link = new CardLink(new HashSet<GameCard>(validCards), Effect);
            Effect.AddCardLink(link);
            ServerPlayer.ServerNotifier.AddCardLink(link);
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