
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Helpers;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace KompasCore.GameCore
{
	public class GameCardRepository<TSerializableCard, TEffect, TCardController> : CardRepository
		where TSerializableCard : SerializableGameCard
		where TEffect : Effect
		where TCardController : CardController
	{

		[Header("Standard Game")]
		public GameObject CardPrefab;

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
				if (!keywordJsons.ContainsKey(keyword))
					Debug.LogError($"Failed to add {keyword} length {keyword.Length} to {card.cardName}"
					+ $"Not present in {string.Join(", ", keywordJsons.Keys)}");
				var keywordJson = keywordJsons[keyword];
				var eff = JsonConvert.DeserializeObject<T>(keywordJson, cardLoadingSettings);
				eff.arg = card.keywordArgs.Length > index ? card.keywordArgs[index] : 0;
				effects.Add(eff);
			}
			return effects;
		}

		protected delegate TGameCard ConstructCard<TGameCard>(TSerializableCard cardInfo, TEffect[] effects, TCardController ctrl);

		public static string JsonPrettify(string json)
		{
			using (var stringReader = new StringReader(json))
			using (var stringWriter = new StringWriter())
			{
				var jsonReader = new JsonTextReader(stringReader);
				var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
				jsonWriter.WriteToken(jsonReader);
				return stringWriter.ToString();
			}
		}

		protected TGameCard InstantiateGameCard<TGameCard>(string json, ConstructCard<TGameCard> cardConstructor, Action<SerializableCard> validation = null)
			where TGameCard : GameCard
		{
			Debug.Log($"Loading {JsonPrettify(json)}");
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
			var card = cardConstructor(cardInfo, effects.ToArray(), ctrl);
            cardObj.name = card.CardName + card.ID;
            return card;
        }
	}
}