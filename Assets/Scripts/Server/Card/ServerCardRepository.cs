using KompasCore.Cards;
using KompasCore.GameCore;
using KompasServer.Effects;
using KompasServer.GameCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
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


        public AvatarServerGameCard InstantiateServerAvatar(string cardName, ServerPlayer owner, int id)
        {
            if (!cardJsons.ContainsKey(cardName)) return null;
            ServerSerializableCard card;
            List<ServerEffect> effects = new List<ServerEffect>();

            try
            {
                card = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[cardName], cardLoadingSettings);
                effects.AddRange(card.effects);
                effects.AddRange(GetKeywordEffects<ServerEffect>(card));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message}, stack trace:\n" +
                    $"{argEx.StackTrace}\nJson was {cardJsons[cardName]}");
                return null;
            }

            //Destroy card components irrelevant to a server avatar
            var avatarObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<ServerCardController>(avatarObj);

            var avatar = new AvatarServerGameCard(card, id, ctrl, owner, effects.ToArray());

            avatar.ServerCardController.serverCard = avatar;
            return avatar;
        }

        public ServerGameCard InstantiateServerNonAvatar(string name, ServerPlayer owner, int id)
        {
            string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
            var cardObj = Instantiate(CardPrefab);
            ServerSerializableCard cardInfo;
            List<ServerEffect> effects = new List<ServerEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[name], cardLoadingSettings);
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
            var card = new ServerGameCard(cardInfo, id, ctrl, owner, effects.ToArray());
            return card;
        }
    }
}