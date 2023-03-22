using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.Effects.Subeffects;
using KompasServer.GameCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KompasServer.Cards
{
    public class ServerCardRepository : GameCardRepository
    {
        public bool CardNameIsCharacter(string name)
        {
            if (!CardExists(name)) return false;

            var card = JsonConvert.DeserializeObject<SerializableCard>(cardJsons[name],
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            return card.cardType == 'C';
        }

        public ServerSubeffect[] InstantiateServerPartialKeyword(string keyword)
        {
            if (!partialKeywordJsons.ContainsKey(keyword))
            {
                Debug.LogError($"No partial keyword json found for {keyword}");
                return new ServerSubeffect[0];
            }

            return JsonConvert.DeserializeObject<ServerSubeffect[]>(partialKeywordJsons[keyword], cardLoadingSettings);
        }
        
        public static TriggerRestrictionElement[] InstantiateTriggerKeyword(string keyword)
        {
            if (!triggerKeywordJsons.ContainsKey(keyword))
            {
                Debug.LogError($"No trigger keyword json found for {keyword}");
                return new TriggerRestrictionElement[0];
            }
            try
            {
                return JsonConvert.DeserializeObject<TriggerRestrictionElement[]>(triggerKeywordJsons[keyword], cardLoadingSettings);
            }
            catch (JsonReaderException)
            {
                Debug.LogError($"Failed to instantiate {keyword}");
                throw;
            }
        }

        //TODO catch argument exception - perhaps a child class? - and return null
        public AvatarServerGameCard InstantiateServerAvatar(string cardName, ServerPlayer owner, int id)
        {
            if (!cardJsons.ContainsKey(cardName)) return null;
            string json = cardJsons[cardName];

            return InstantiateGameCard<AvatarServerGameCard>(json, (cardInfo, effects, ctrl) => new AvatarServerGameCard(cardInfo, id, ctrl, owner, effects));
        }

        private delegate T ConstructCard<T>(ServerSerializableCard cardInfo, ServerEffect[] effects, ServerCardController ctrl)
            where T : ServerGameCard;

        public ServerGameCard InstantiateServerNonAvatar(string name, ServerPlayer owner, int id)
        {
            string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
            return InstantiateGameCard<ServerGameCard>(json, (cardInfo, effects, ctrl) => new ServerGameCard(cardInfo, id, ctrl, owner, effects));
        }

        private T InstantiateGameCard<T>(string json, ConstructCard<T> cardConstructor, Action<SerializableCard> validation = null)
            where T : ServerGameCard
        {
            var cardObj = Instantiate(CardPrefab);
            ServerSerializableCard cardInfo;
            List<ServerEffect> effects = new List<ServerEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[name], cardLoadingSettings);
                validation?.Invoke(cardInfo);
                effects.AddRange(cardInfo.effects);
                effects.AddRange(GetKeywordEffects<ServerEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, stacktrace {argEx.StackTrace}");
                return null;
            }

            var ctrl = GetCardController<ServerCardController>(cardObj);
            return cardConstructor(cardInfo, effects.ToArray(), ctrl);
        }
    }
}