
using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.GameCore;
using KompasDeckbuilder.UI;
using Newtonsoft.Json;
using UnityEngine;

namespace KompasDeckbuilder
{
    public class DeckBuilderCardRepository : CardRepository
    {
        public GameObject deckBuilderCardPrefab;

        public static IEnumerable<SerializableCard> SerializableCards => GetSerializableCards(CardJsons);
        private static IEnumerable<SerializableCard> GetSerializableCards(IEnumerable<string> jsons)
            => jsons.Select(json => SerializableCardFromJson(json)).Where(card => card != null);
        private static SerializableCard SerializableCardFromJson(string json)
        {
            try
            {
                //Debug.Log($"Deserializing {json}");
                return JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
            }
            catch (System.ArgumentException e)
            {
                Debug.LogError($"{json} had argument exception {e.Message}");
            }
            return null;
        }
        
        // new version
        public KompasDeckbuilder.UI.DeckBuilderCardController InstantiateDeckBuilderCard(string json, DeckBuilderController deckBuilderController)
        {
            SerializableCard serializableCard = SerializableCardFromJson(json);
            if (serializableCard == null) return null;

            var card = Instantiate(deckBuilderCardPrefab).GetComponent<KompasDeckbuilder.UI.DeckBuilderCardController>();
            card.SetInfo(serializableCard, deckBuilderController, FileNameFor(serializableCard.cardName));
            return card;
        }
    }
}