using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.GameCore;
using Newtonsoft.Json;
using System.Linq;
using UnityEngine;

namespace KompasClient.Cards
{
	public class ClientCardRepository : GameCardRepository<ClientSerializableCard, ClientEffect, ClientCardController>
	{
		public GameObject DeckSelectCardPrefab;

		public Material friendlyCardMaterial;
		public Material enemyCardMaterial;

		public AvatarClientGameCard InstantiateClientAvatar(string json, ClientPlayer owner, int id)
		{
			void validation(SerializableCard cardInfo)
			{
				if (cardInfo.cardType != 'C') throw new System.NotImplementedException("Card type for client avatar isn't character!");
			}

			AvatarClientGameCard ConstructAvatar(ClientSerializableCard cardInfo, ClientEffect[] effects, ClientCardController ctrl)
				=> new(cardInfo, owner, effects, id, ctrl);

			return InstantiateGameCard(json, ConstructAvatar, validation);
		}

		public ClientGameCard InstantiateClientNonAvatar(string json, ClientPlayer owner, int id)
		{
			var card = InstantiateGameCard(json,
				(cardInfo, effects, ctrl) => new ClientGameCard(cardInfo, id, owner, effects, ctrl));

			card.ClientCardController.gameCardViewController.cardModelController.SetFrameMaterial(owner.Friendly ? friendlyCardMaterial : enemyCardMaterial);
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