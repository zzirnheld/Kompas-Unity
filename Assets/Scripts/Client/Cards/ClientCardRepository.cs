using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasClient.Cards
{
    public class ClientCardRepository : CardRepository
    {
        public GameObject DeckSelectCardPrefab;

        public AvatarClientGameCard InstantiateClientAvatar(string json, ClientPlayer owner, int id)
        {
            ClientSerializableCard cardInfo;
            List<ClientEffect> effects = new List<ClientEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<ClientSerializableCard>(json, cardLoadingSettings);
                if (cardInfo.cardType != 'C') throw new System.NotImplementedException("Card type for client avatar isn't character!");

                effects.AddRange(cardInfo.effects);
                effects.AddRange(GetKeywordEffects<ClientEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load client Avatar, argument exception with message {argEx.Message},\n {argEx.StackTrace}, for json:\n{json}");
                return null;
            }

            var avatarObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<ClientCardController>(avatarObj);
            return new AvatarClientGameCard(cardInfo, owner, effects.ToArray(), id, ctrl);
        }

        public ClientGameCard InstantiateClientNonAvatar(string json, ClientPlayer owner, int id)
        {
            ClientSerializableCard cardInfo;
            List<ClientEffect> effects = new List<ClientEffect>();

            try
            {
                cardInfo = JsonConvert.DeserializeObject<ClientSerializableCard>(json, cardLoadingSettings);
                effects.AddRange(cardInfo.effects);
                effects.AddRange(GetKeywordEffects<ClientEffect>(cardInfo));
            }
            catch (System.ArgumentException argEx)
            {
                //Catch JSON parse error
                Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, {argEx.StackTrace}");
                return null;
            }

            var cardObj = Instantiate(CardPrefab);
            var ctrl = GetCardController<ClientCardController>(cardObj);
            var card = new ClientGameCard(cardInfo, id, owner, effects.ToArray(), ctrl);

            Debug.Log($"Successfully created a card? {card != null} for json {json} with controller {ctrl}");
            card.ClientCardController.gameCardViewController.Refresh();

            //handle adding existing card links
            foreach (var c in card.Game.Cards)
            {
                foreach (var link in c.CardLinkHandler.Links)
                {
                    if (link.CardIDs.Contains(id)) card.CardLinkHandler.AddLink(link);
                }
            }

            return card;
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