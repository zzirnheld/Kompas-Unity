using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.GameCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.Cards
{
    public class ClientCardRepository : GameCardRepository
    {
        public GameObject DeckSelectCardPrefab;

        public AvatarClientGameCard InstantiateClientAvatar(string json, ClientPlayer owner, int id)
        {
            Action<SerializableCard> validation = cardInfo =>
            {
                if (cardInfo.cardType != 'C') throw new System.NotImplementedException("Card type for client avatar isn't character!");
            };

            return InstantiateGameCard<AvatarClientGameCard>(json, (cardInfo, effects, ctrl) => new AvatarClientGameCard(cardInfo, owner, effects, id, ctrl),
                validation);
        }

        public ClientGameCard InstantiateClientNonAvatar(string json, ClientPlayer owner, int id)
        {
            var card = InstantiateGameCard<ClientGameCard>(json, (cardInfo, effects, ctrl) => new ClientGameCard(cardInfo, id, owner, effects, ctrl));

            card.ClientCardController.gameCardViewController.Refresh();

            //handle adding existing card links
            foreach (var c in card.Game.Cards.ToArray())
            {
                foreach (var link in c.CardLinkHandler.Links.ToArray())
                {
                    if (link.CardIDs.Contains(id)) card.CardLinkHandler.AddLink(link);
                }
            }

            return card;
        }

        private delegate T ConstructCard<T>(ClientSerializableCard cardInfo, ClientEffect[] effects, ClientCardController ctrl)
            where T : ClientGameCard;

        private T InstantiateGameCard<T>(string json, ConstructCard<T> cardConstructor, Action<SerializableCard> validation = null)
            where T : ClientGameCard
        {
            ClientSerializableCard cardInfo;
            var effects = new List<ClientEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<ClientSerializableCard>(json, cardLoadingSettings);
                validation?.Invoke(cardInfo);

                effects.AddRange(cardInfo.effects);
                effects.AddRange(GetKeywordEffects<ClientEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx) //Catch JSON parse error
            {
                Debug.LogError($"Failed to load client card, argument exception with message {argEx.Message},\n {argEx.StackTrace}, for json:\n{json}");
                return null;
            }

            var avatarObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<ClientCardController>(avatarObj);
            return cardConstructor(cardInfo, effects.ToArray(), ctrl);
        }

        public DeckSelectCardController InstantiateDeckSelectCard(string json, Transform parent, DeckSelectCardController prefab, DeckSelectUIController uiCtrl)
        {
            try
            {
                SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
                DeckSelectCardController card = Instantiate(prefab, parent);
                card.SetInfo(serializableCard, uiCtrl, cardFileNames[serializableCard.cardName]);
                return card;
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}");
                return null;
            }
        }
    }
}