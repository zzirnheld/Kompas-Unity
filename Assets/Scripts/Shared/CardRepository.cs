﻿using KompasClient.Cards;
using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions;
using KompasDeckbuilder;
using KompasDeckbuilder.UI;
using KompasServer.Cards;
using KompasServer.Effects;
using KompasServer.GameCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO split into client/server
public class CardRepository : MonoBehaviour
{
    public const string cardJsonsFolderPath = "Card Jsons/";
    public const string cardListFilePath = cardJsonsFolderPath + "Card List";

    public const string keywordJsonsFolderPath = "Keyword Jsons/Full Keywords/";
    public const string keywordListFilePath = keywordJsonsFolderPath + "Keyword List";

    public const string partialKeywordFolderPath = "Keyword Jsons/Partial Keywords/";
    public const string partialKeywordListFilePath = partialKeywordFolderPath + "Keyword List";

    public const string triggerKeywordFolderPath = "Keyword Jsons/Trigger Keywords/";
    public const string triggerKeywordListFilePath = triggerKeywordFolderPath + "Keyword List";

    public const string RemindersJsonPath = "Reminder Text/Reminder Texts";

    protected static readonly JsonSerializerSettings cardLoadingSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        MaxDepth = null,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };

    private static readonly string[] cardNamesToIgnore = new string[]
    {
        "Square Kompas Logo"
    };

    protected static readonly Dictionary<string, string> cardJsons = new Dictionary<string, string>();
    protected static readonly Dictionary<string, string> cardFileNames = new Dictionary<string, string>();
    private static readonly Dictionary<string, int> cardNameIDs = new Dictionary<string, int>();
    private static readonly List<string> cardNames = new List<string>();

    protected static readonly Dictionary<string, string> keywordJsons = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> partialKeywordJsons = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> triggerKeywordJsons = new Dictionary<string, string>();

    public static ReminderTextsContainer Reminders { get; private set; }
    public static ICollection<string> Keywords { get; private set; }

    private static bool initalized = false;

    public static IEnumerable<string> CardJsons => cardJsons.Values;

    #region prefabs
    public GameObject DeckSelectCardPrefab;

    public GameObject DeckbuilderCharPrefab;
    public GameObject DeckbuilderSpellPrefab;
    public GameObject DeckbuilderAugPrefab;

    public GameObject CardPrefab;
    #endregion prefabs

    private void Awake()
    {
        if (initalized) return;
        initalized = true;

        InitializeCardJsons();

        InitializeMapFromJsons(keywordListFilePath, keywordJsonsFolderPath, keywordJsons);
        InitializeMapFromJsons(partialKeywordListFilePath, partialKeywordFolderPath, partialKeywordJsons);
        InitializeMapFromJsons(triggerKeywordListFilePath, triggerKeywordFolderPath, triggerKeywordJsons);

        var reminderJsonAsset = Resources.Load<TextAsset>(RemindersJsonPath);
        Reminders = JsonConvert.DeserializeObject<ReminderTextsContainer>(reminderJsonAsset.text);
        Reminders.Initialize();
        Keywords = Reminders.keywordReminderTexts.Select(rti => rti.keyword).ToArray();
    }

    private void InitializeCardJsons()
    {
        static bool isCardToIgnore(string name) => string.IsNullOrWhiteSpace(name) || cardNamesToIgnore.Contains(name);

        string cardFilenameList = Resources.Load<TextAsset>(cardListFilePath).text;
        cardFilenameList = cardFilenameList.Replace('\r', '\n');
        string[] cardFilenameArray = cardFilenameList.Split('\n');

        foreach (string filename in cardFilenameArray)
        {
            if (string.IsNullOrEmpty(filename)) continue;
            //sanitize the filename. for some reason, doing substring fixes stuff
            string filenameClean = filename.Substring(0, filename.Length);
            //don't add duplicate cards
            if (isCardToIgnore(filenameClean) || CardExists(filenameClean)) continue;

            //load the json
            var jsonAsset = Resources.Load<TextAsset>(cardJsonsFolderPath + filenameClean);
            if (jsonAsset == null)
            {
                Debug.LogError($"Failed to load json file for {filenameClean}");
                continue;
            }
            string json = jsonAsset.text;
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");
            //load the cleaned json to get the card's name according to itself
            SerializableCard card;
            try
            {
                card = JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"Failed to load {json}. Error\n{e}");
                continue;
            }
            string cardName = card.cardName;

            //add the cleaned json to the dictionary
            cardJsons.Add(cardName, json);
            cardFileNames.Add(cardName, filename);
            //Debug.Log($"Entry for {cardName} is {filenameClean}");
            //add the card's name to the list of card names
            cardNameIDs.Add(cardName, cardNames.Count);
            cardNames.Add(cardName);
        }
    }

    private void InitializeMapFromJsons(string filePath, string folderPath, Dictionary<string, string> dict)
    {
        string keywordList = Resources.Load<TextAsset>(filePath).text;
        var keywords = keywordList.Replace('\r', '\n').Split('\n').Where(s => !string.IsNullOrEmpty(s));
        foreach (string keyword in keywords)
        {
            string json = Resources.Load<TextAsset>(folderPath + keyword).text;
            dict.Add(keyword, json);
        }
    }

    protected List<T> GetKeywordEffects<T>(SerializableCard card) where T : Effect
    {
        List<T> effects = new List<T>();
        for (int i = 0; i < card.keywords.Length; i++)
        {
            var s = card.keywords[i];
            var keywordJson = keywordJsons[s];
            var eff = JsonConvert.DeserializeObject<T>(keywordJson, cardLoadingSettings);
            eff.arg = card.keywordArgs.Length > i ? card.keywordArgs[i] : 0;
            effects.Add(eff);
        }
        return effects;
    }

    protected T GetCardController<T>(GameObject gameObject) where T : CardController
    {
        var cardCtrlComponents = gameObject
            .GetComponents(typeof(CardController))
            .Where(c => !(c is T));
        foreach (var c in cardCtrlComponents) Destroy(c);

        //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
        return gameObject.GetComponents<T>().Where(c => c is T).First();
    }
    public static bool CardExists(string cardName) => cardNameIDs.ContainsKey(cardName);

    public string GetJsonFromName(string name)
    {
        if (!cardJsons.ContainsKey(name))
        {
            Debug.LogError($"No json found for name \"{name ?? "null"}\" of length {name?.Length ?? 0}");
            return null;
        }

        return cardJsons[name];
    }

    public IEnumerable<string> GetJsonsFromNames(IEnumerable<string> names)
        => names.Select(n => GetJsonFromName(n)).Where(json => json != null);

    public ServerSubeffect[] InstantiateServerPartialKeyword(string keyword)
    {
        if (!partialKeywordJsons.ContainsKey(keyword))
        {
            Debug.Log($"No partial keword json found for {keyword}");
            return new ServerSubeffect[0];
        }

        return JsonConvert.DeserializeObject<ServerSubeffect[]>(partialKeywordJsons[keyword], cardLoadingSettings);
    }

    #region Create Cards
    public bool CardNameIsCharacter(string name)
    {
        if (!CardExists(name)) return false;

        var card = JsonConvert.DeserializeObject<SerializableCard>(cardJsons[name],
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        return card.cardType == 'C';
    }

    public DeckbuilderCardController InstantiateDeckbuilderCard(string json, DeckbuildSearchController searchCtrl, bool inDeck)
    {
        try
        {
            SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json, cardLoadingSettings);
            var card = Instantiate(DeckbuilderCharPrefab).GetComponent<DeckbuilderCardController>();
            card.SetInfo(searchCtrl, serializableCard, inDeck);
            card.FileName = cardFileNames[card.Card.CardName];
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception {argEx}");
            return null;
        }
    }


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


    public static TriggerRestrictionElement[] InstantiateTriggerKeyword(string keyword)
    {
        if (!partialKeywordJsons.ContainsKey(keyword))
        {
            Debug.Log($"No partial keword json found for {keyword}");
            return new TriggerRestrictionElement[0];
        }

        return JsonConvert.DeserializeObject<TriggerRestrictionElement[]>(triggerKeywordJsons[keyword], cardLoadingSettings);
    }

    public static string FileNameFor(string cardName) => cardFileNames[cardName];
    #endregion Create Cards
}
