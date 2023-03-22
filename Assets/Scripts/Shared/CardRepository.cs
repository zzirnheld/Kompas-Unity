using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions;
using KompasCore.Helpers;
using KompasDeckbuilder;
using KompasDeckbuilder.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using KompasServer.Effects.Subeffects;

namespace KompasCore.GameCore
{
    public class CardRepository : MonoBehaviour
    {
        public const string cardJsonsFolderPath = "Card Jsons";
        public static readonly string cardListFilePath = Path.Combine(cardJsonsFolderPath, "Card List");

        public static readonly string keywordJsonsFolderPath = Path.Combine("Keyword Jsons", "Full Keywords");
        public static readonly string keywordListFilePath = Path.Combine(keywordJsonsFolderPath, "Keyword List");

        public static readonly string partialKeywordFolderPath = Path.Combine("Keyword Jsons", "Partial Keywords");
        public static readonly string partialKeywordListFilePath = Path.Combine(partialKeywordFolderPath, "Keyword List");

        public static readonly string triggerKeywordFolderPath = Path.Combine("Keyword Jsons", "Trigger Keywords");
        public static readonly string triggerKeywordListFilePath = Path.Combine(triggerKeywordFolderPath, "Keyword List");

        public static readonly string RemindersJsonPath = Path.Combine("Reminder Text", "Reminder Texts");

        private static readonly Regex subeffRegex = new Regex(@"Subeffect:([^:]+):"); //Subeffect:*:
        private const string subeffReplacement = @"KompasServer.Effects.Subeffects.$1, Assembly-CSharp";

        //restriction regexes
        private static readonly Regex coreRestrictionRegex = new Regex(@"Core\.([^R]+)Restriction:([^:]+):"); //Core.*Restriction:*:
        private const string coreRestrictionReplacement = @"KompasCore.Effects.Restrictions.$1RestrictionElements.$2, Assembly-CSharp";

        //identity regexes
        private static readonly Regex cardsIdentityRegex = new Regex(@"""Cards:([^:]+):"); //"Cards:*:
        private const string cardsIdentityReplacement = @"""KompasCore.Effects.Identities.Cards.$1, Assembly-CSharp";

        private static readonly Regex manyCardsIdentityRegex = new Regex(@"""ManyCards:([^:]+):"); //"ManyCards:*:
        private const string manyCardsIdentityReplacement = @"""KompasCore.Effects.Identities.ManyCards.$1, Assembly-CSharp";

        private static readonly Regex spacesIdentityRegex = new Regex(@"""Spaces:([^:]+):"); //"Spaces:*:
        private const string spacesIdentityReplacement = @"""KompasCore.Effects.Identities.Spaces.$1, Assembly-CSharp";

        private static readonly Regex manySpacesIdentityRegex = new Regex(@"""ManySpaces:([^:]+):"); //"ManySpaces:*:
        private const string manySpacesIdentityReplacement = @"""KompasCore.Effects.Identities.ManySpaces.$1, Assembly-CSharp";

        private static readonly Regex numbersIdentityRegex = new Regex(@"""Numbers:([^:]+):"); //"Numbers:*:
        private const string numbersIdentityReplacement = @"""KompasCore.Effects.Identities.Numbers.$1, Assembly-CSharp";

        private static readonly Regex manyNumbersIdentityRegex = new Regex(@"""ManyNumbers:([^:]+):"); //"ManyNumbers:*:
        private const string manyNumbersIdentityReplacement = @"""KompasCore.Effects.Identities.ManyNumbers.$1, Assembly-CSharp";

        private static readonly Regex playersIdentityRegex = new Regex(@"""Players:([^:]+):"); //"Players:*:
        private const string playersIdentityReplacement = @"""KompasCore.Effects.Identities.Players.$1, Assembly-CSharp";

        private static readonly Regex stackablesIdentityRegex = new Regex(@"""Stackables:([^:]+):"); //"Stackables:*:
        private const string stackablesIdentityReplacement = @"""KompasCore.Effects.Identities.Stackables.$1, Assembly-CSharp";

        //relationships
        private static readonly Regex relationshipRegex = new Regex(@"Relationships\.([^:]+):([^:]+):"); //Relationships.*:*:
        private const string relationshipReplacement = @"KompasCore.Effects.Relationships.$1Relationships.$2, Assembly-CSharp";

        private static readonly Regex numberSelectorRegex = new Regex(@"NumberSelector:([^:]+):"); //NumberSelector:*:
        private const string numberSelectorReplacement = @"KompasCore.Effects.Identities.NumberSelectors.$1, Assembly-CSharp";

        private static readonly Regex threeSpaceRelationshipRegex = new Regex(@"ThreeSpaceRelationships:([^:]+):"); //ThreeSpaceRelationships:*:
        private const string threeSpaceRelationshipReplacement = @"KompasCore.Effects.Identities.ThreeSpaceRelationships.$1, Assembly-CSharp";

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
        private static IReadOnlyCollection<string> CardNames => cardJsons.Keys;

        protected static readonly Dictionary<string, string> keywordJsons = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> partialKeywordJsons = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> triggerKeywordJsons = new Dictionary<string, string>();

        public static ReminderTextsContainer Reminders { get; private set; }
        public static ICollection<string> Keywords { get; private set; }

        private static bool initalized = false;
        private static readonly object initializationLock = new object();

        public static IEnumerable<string> CardJsons => cardJsons.Values;

        [Header("Old Deck Builder")]
        public GameObject DeckbuilderCharPrefab;

        [Header("New (WIP) Deck Builder")]
        public GameObject deckBuilderCardPrefab;

        [Header("Standard Game")]
        public GameObject CardPrefab;

        public static void Init() => InitializeCardJsons();

        private void Awake()
        {
            lock (initializationLock)
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
        }

        private static void InitializeCardJsons()
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
                var jsonAsset = Resources.Load<TextAsset>(Path.Combine(cardJsonsFolderPath, filenameClean));
                if (jsonAsset == null)
                {
                    Debug.LogError($"Failed to load json file for {filenameClean}");
                    continue;
                }
                string json = jsonAsset.text;

                //handle tags like subeffs, etc.
                json = ReplacePlaceholders(json);

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
                //if this throws a key existing exception, you probably have two cards with the same name field, but diff file names
                if (cardJsons.ContainsKey(cardName)) continue;
                cardJsons.Add(cardName, json);
                cardFileNames.Add(cardName, filename);
            }

            Debug.Log(string.Join("\n", CardNames));
        }

        private void InitializeMapFromJsons(string filePath, string folderPath, Dictionary<string, string> dict)
        {
            string keywordList = Resources.Load<TextAsset>(filePath).text;
            var keywords = keywordList.Replace('\r', '\n').Split('\n').Where(s => !string.IsNullOrEmpty(s));
            Debug.Log($"Keywords list: \n{string.Join("\n", keywords.Select(keyword => $"{keyword} length {keyword.Length}"))}");
            foreach (string keyword in keywords)
            {
                Debug.Log($"Loading {keyword} from {Path.Combine(folderPath, keyword)}");
                string json = Resources.Load<TextAsset>(Path.Combine(folderPath, keyword)).text;
                json = ReplacePlaceholders(json);
                dict.Add(keyword, json);
            }
        }

        protected List<T> GetKeywordEffects<T>(SerializableCard card) where T : Effect
        {
            List<T> effects = new List<T>();
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

        private static string ReplacePlaceholders(string json)
        {
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");

            json = subeffRegex.Replace(json, subeffReplacement);

            json = coreRestrictionRegex.Replace(json, coreRestrictionReplacement);

            //Many before single, to not replace the many with a broken thing
            json = manyCardsIdentityRegex.Replace(json, manyCardsIdentityReplacement);
            json = cardsIdentityRegex.Replace(json, cardsIdentityReplacement);

            json = manySpacesIdentityRegex.Replace(json, manySpacesIdentityReplacement);
            json = spacesIdentityRegex.Replace(json, spacesIdentityReplacement);

            json = manyNumbersIdentityRegex.Replace(json, manyNumbersIdentityReplacement);
            json = numbersIdentityRegex.Replace(json, numbersIdentityReplacement);

            json = playersIdentityRegex.Replace(json, playersIdentityReplacement);
            json = stackablesIdentityRegex.Replace(json, stackablesIdentityReplacement);

            json = relationshipRegex.Replace(json, relationshipReplacement);
            json = numberSelectorRegex.Replace(json, numberSelectorReplacement);
            json = threeSpaceRelationshipRegex.Replace(json, threeSpaceRelationshipReplacement);

            return json;
        }

        public static bool CardExists(string cardName) => CardNames.Contains(cardName);

        public string GetJsonFromName(string name)
        {
            if (!cardJsons.ContainsKey(name))
            {
                //This log exists exclusively for debugging purposes
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
                Debug.LogError($"No partial keyword json found for {keyword}");
                return new ServerSubeffect[0];
            }

            return JsonConvert.DeserializeObject<ServerSubeffect[]>(partialKeywordJsons[keyword], cardLoadingSettings);
        }

        #region Create Cards
        // new version
        public KompasDeckbuilder.UI.DeckBuilderCardController InstantiateDeckBuilderCard(string json, DeckBuilderController deckBuilderController)
        {
            SerializableCard serializableCard = SerializableCardFromJson(json);
            if (serializableCard == null) return null;

            var card = Instantiate(deckBuilderCardPrefab).GetComponent<KompasDeckbuilder.UI.DeckBuilderCardController>();
            card.SetInfo(serializableCard, deckBuilderController, FileNameFor(serializableCard.cardName));
            return card;
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

        public static string FileNameFor(string cardName) => cardFileNames[cardName];
        #endregion Create Cards


        public static Sprite LoadSprite(string cardFileName) => Resources.Load<Sprite>(Path.Combine("Simple Sprites", cardFileName));
    }
}