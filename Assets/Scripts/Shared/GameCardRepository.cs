
using System;
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Helpers;
using Newtonsoft.Json;
using UnityEngine;

namespace KompasCore.GameCore
{
    public class GameCardRepository<PSerializableCard, PEffect, PCardController> : CardRepository
        where PSerializableCard : SerializableCard
        where PEffect : Effect
        where PCardController : CardController
    {
        [Header("Standard Game")]
        public GameObject CardPrefab;

        protected T GetCardController<T>(GameObject gameObject) where T : CardController
        {
            var cardCtrlComponents = gameObject
                .GetComponents(typeof(CardController))
                .Where(c => !(c is T));
            foreach (var c in cardCtrlComponents) Destroy(c);

            //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
            return gameObject.GetComponents<T>().Where(c => c is T).First();
        }

        protected IList<T> GetKeywordEffects<T>(SerializableCard card) where T : Effect
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

        protected delegate T ConstructCard<T>(PSerializableCard cardInfo, PEffect[] effects, PCardController ctrl);

        protected T InstantiateGameCard<T>(string json, ConstructCard<T> cardConstructor, Action<SerializableCard> validation = null)
        {
            PSerializableCard cardInfo;
            var effects = new List<PEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<PSerializableCard>(cardJsons[name], cardLoadingSettings);
                validation?.Invoke(cardInfo);
                
                foreach (var eff in cardInfo.Effects)
                {
                    if (eff is PEffect pEffect) effects.Add(pEffect);
                    else throw new System.ArgumentException($"Mismatch between type of effect {eff.GetType()} and type parameter for card creation {typeof(PEffect)}");
                }
                effects.AddRange(GetKeywordEffects<PEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, stacktrace {argEx.StackTrace}");
                return default;
            }

            var cardObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<PCardController>(cardObj);
            return cardConstructor(cardInfo, effects.ToArray(), ctrl);
        }
    }
}