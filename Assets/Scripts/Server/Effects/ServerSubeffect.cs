using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasServer.GameCore;
using UnityEngine;

namespace KompasServer.Effects
{
    [System.Serializable]
    public abstract class ServerSubeffect : Subeffect
    {
        public override Player Controller => EffectController;
        public override Effect Effect => ServerEffect;
        public override Game Game => ServerGame;

        public ServerEffect ServerEffect { get; protected set; }
        public ServerGame ServerGame => ServerEffect.serverGame;
        public ServerPlayer EffectController => ServerEffect.ServerController;
        public GameCard ThisCard => ServerEffect.Source;

        public ServerPlayer ServerPlayer 
            => ServerGame.ServerPlayers[(Controller.index + playerIndex) % ServerGame.ServerPlayers.Length];

        private static ServerSubeffect FromJson(string subeffType, string subeffJson)
        {
            var type = System.Type.GetType(subeffType);
            return JsonUtility.FromJson(subeffJson, type) as ServerSubeffect;
        }

        public static ServerSubeffect FromJson(string subeffJson, ServerEffect parent, int subeffIndex)
        {
            Debug.Log($"Trying to load subeffect parent from {subeffJson}");

            var subeff = JsonUtility.FromJson<Subeffect>(subeffJson);

            Debug.Log($"Creating subeffect from json of type {subeff.subeffType} json {subeffJson}");

            var toReturn = FromJson(subeff.subeffType, subeffJson);
            toReturn?.Initialize(parent, subeffIndex);
            return toReturn;
        }

        /// <summary>
        /// Server Subeffect resolve method. Does whatever this type of subeffect does,
        /// then executes the next subeffect.<br></br>
        /// Usually, this involves a call to ResolveNextSubeffect(),
        /// but if the subeffect does control flow, it calles ResolveSubeffect for a specific thing.<br></br>
        /// TODO refactor to return an enum/int to represent different control flow states (negatives being special values):<br></br>
        ///     effect impossible, 
        ///     resolve next, 
        ///     resolve specific, 
        ///     awaiting player input.<br></br>
        /// <returns><see langword="true"/> if the effect finished resolving successfully, <see langword="false"/> if it's awaiting response</returns>
        /// </summary>
        public abstract bool Resolve();

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
        /// Optional method. If implemented, does something when the effect is declared impossible.
        /// Default implementation just finishes resolution of the effect
        /// </summary>
        public virtual bool OnImpossible()
        {
            Debug.Log($"Base On Impossible called for {GetType()}");
            ServerEffect.OnImpossible = null;
            return ServerEffect.EffectImpossible();
        }
    }
}