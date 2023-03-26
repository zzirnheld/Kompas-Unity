
using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Helpers;
using Newtonsoft.Json;
using UnityEngine;
using static VoxelCard;

namespace KompasCore.GameCore
{
    public class GameCardRepository<TSerializableCard, TEffect, TCardController> : CardRepository
        where TSerializableCard : SerializableGameCard
        where TEffect : Effect
        where TCardController : CardController
    {
        [Header("Standard Game")]
        public GameObject CardPrefab;

        //TODO next: maintain a static list of recalculated textures, and grab them from there rather than recalculating for each new copy of a card created

        private static IDictionary<string, (Texture2D, Texture2D, Texture2D, Texture2D)> cardFileNameToTextures;
        private static Texture2D friendlyZoomedCharTexture;
        private static Texture2D friendlyZoomedNonCharTexture;
        private static Texture2D friendlyUnzoomedCharTexture;
        private static Texture2D friendlyUnzoomedNonCharTexture;
        private static Texture2D friendlyZoomedCharMetalness;
        private static Texture2D friendlyZoomedNonCharMetalness;
        private static Texture2D friendlyUnzoomedCharMetalness;
        private static Texture2D friendlyUnzoomedNonCharMetalness;

        private static Texture2D enemyZoomedCharTexture;
        private static Texture2D enemyZoomedNonCharTexture;
        private static Texture2D enemyUnzoomedCharTexture;
        private static Texture2D enemyUnzoomedNonCharTexture;
        private static Texture2D enemyZoomedCharMetalness;
        private static Texture2D enemyZoomedNonCharMetalness;
        private static Texture2D enemyUnzoomedCharMetalness;
        private static Texture2D enemyUnzoomedNonCharMetalness;

        public Sprite frameTexture;
        public Sprite namePlacardTexture;
        public Sprite typePlacardTexture;
        public Sprite effectTextTexture;
        public Sprite cardBackTexture;
        public Sprite nTexture;
        public Sprite eTexture;
        public Sprite sacTexture;
        public Sprite wTexture;
        public Sprite rTexture;
        public Sprite dTexture;

        private static bool initialized = false;
        private static object initializationLock = new object();

        public Game game;
        public Settings Settings
        {
            get
            {
                if (game != null) return game.Settings;
                else return default;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            lock (initializationLock)
            {
                if (initialized) return;

                var textures = new TextureParams.Textures()
                {
                    NamePlacardTexture = namePlacardTexture,
                    TypePlacardTexture = typePlacardTexture,
                    EffectTextTexture = effectTextTexture,
                    CardBackTexture = cardBackTexture,
                    NTexture = nTexture,
                    ETexture = eTexture,
                    SACTexture = sacTexture,
                    WTexture = wTexture,
                    RTexture = rTexture,
                    DTexture = dTexture,
                };

                textures.FrameColorOverride = Settings?.FriendlyColor ?? Settings.DefaultFriendlyBlue;
                (friendlyZoomedCharTexture, friendlyZoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: true, textures), true);
                (friendlyZoomedNonCharTexture, friendlyZoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: false, textures), true);
                (friendlyUnzoomedCharTexture, friendlyUnzoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: true, textures), true);
                (friendlyUnzoomedNonCharTexture, friendlyUnzoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: false, textures), true);

                textures.FrameColorOverride = Settings?.EnemyColor ?? Settings.DefaultEnemyRed;
                (enemyZoomedCharTexture, enemyZoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: true, textures), true);
                (enemyZoomedNonCharTexture, enemyZoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: false, textures), true);
                (enemyUnzoomedCharTexture, enemyUnzoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: true, textures), true);
                (enemyUnzoomedNonCharTexture, enemyUnzoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: false, textures), true);
            }
        }

        private T GetCardController<T>(GameObject gameObject) where T : TCardController //Prevents casting
        {
            var cardCtrlComponents = gameObject
                .GetComponents(typeof(CardController))
                .Where(c => !(c is T));
            foreach (var c in cardCtrlComponents) Destroy(c);

            //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
            return gameObject.GetComponents<T>().Where(c => c is T).First();
        }

        private IList<T> GetKeywordEffects<T>(SerializableCard card) where T : TEffect //Prevents casting
        {
            var effects = new List<T>();
            foreach (var (index, keyword) in card.keywords.Enumerate())
            {
                if (!keywordJsons.ContainsKey(keyword)) Debug.LogError($"Failed to add {keyword} length {keyword.Length} to {card.cardName}");
                var keywordJson = keywordJsons[keyword];
                var eff = JsonConvert.DeserializeObject<T>(keywordJson, cardLoadingSettings);
                eff.arg = card.keywordArgs.Length > index ? card.keywordArgs[index] : 0;
                effects.Add(eff);
            }
            return effects;
        }

        protected delegate TGameCard ConstructCard<TGameCard>(TSerializableCard cardInfo, TEffect[] effects, TCardController ctrl);

        protected TGameCard InstantiateGameCard<TGameCard>(string json, ConstructCard<TGameCard> cardConstructor, Action<SerializableCard> validation = null)
        {
            TSerializableCard cardInfo;
            var effects = new List<TEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<TSerializableCard>(json, cardLoadingSettings);
                validation?.Invoke(cardInfo);

                effects.AddRangeWithCast(cardInfo.Effects);
                effects.AddRange(GetKeywordEffects<TEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, stacktrace {argEx.StackTrace}");
                return default;
            }

            var cardObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<TCardController>(cardObj);
            return cardConstructor(cardInfo, effects.ToArray(), ctrl);
        }
    }
}