using KompasCore.Cards;
using KompasDeckbuilder.UI;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using System.IO;
using static VoxelCard;
using UnityEditor;

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
        private const string ZoomedTextureFolder = "Assets/Resources/Card Textures/Zoomed/Texture";
        private const string ZoomedMetalnessFolder = "Assets/Resources/Card Textures/Zoomed/Metalness";
        private const string UnzoomedTextureFolder = "Assets/Resources/Card Textures/Unzoomed/Texture";
        private const string UnzoomedMetalnessFolder = "Assets/Resources/Card Textures/Unzoomed/Metalness";

        public class CardTextures
        {
            public readonly Texture2D zoomedTex;
            public readonly Texture2D zoomedMet;
            public readonly Texture2D unzoomedTex;
            public readonly Texture2D unzoomedMet;

            public CardTextures(Texture2D zoomedTex, Texture2D zoomedMet, Texture2D unzoomedTex, Texture2D unzoomedMet)
            {
                this.zoomedTex = zoomedTex;
                this.zoomedMet = zoomedMet;
                this.unzoomedTex = unzoomedTex;
                this.unzoomedMet = unzoomedMet;
            }

            internal void Deconstruct(out Texture2D zoomedTex, out Texture2D zoomedMet, out Texture2D unzoomedTex, out Texture2D unzoomedMet)
            {
                zoomedTex = this.zoomedTex;
                zoomedMet = this.zoomedMet;
                unzoomedTex = this.unzoomedTex;
                unzoomedMet = this.unzoomedMet;
            }
        }

        private static IDictionary<string, CardTextures> cardFileNameToTextures = new Dictionary<string, CardTextures>();
        private static Texture2D friendlyZoomedCharTexture;
        private static Texture2D friendlyZoomedNonCharTexture;
        private static Texture2D friendlyUnzoomedCharTexture;
        private static Texture2D friendlyUnzoomedNonCharTexture;
        private static Texture2D friendlyZoomedCharMetalness;
        private static Texture2D friendlyZoomedNonCharMetalness;
        private static Texture2D friendlyUnzoomedCharMetalness;
        private static Texture2D friendlyUnzoomedNonCharMetalness;

        private static Texture2D enemyZoomedCharTexture;
        private static Texture2D enemyZoomedNonCharTexture;
        private static Texture2D enemyUnzoomedCharTexture;
        private static Texture2D enemyUnzoomedNonCharTexture;
        private static Texture2D enemyZoomedCharMetalness;
        private static Texture2D enemyZoomedNonCharMetalness;
        private static Texture2D enemyUnzoomedCharMetalness;
        private static Texture2D enemyUnzoomedNonCharMetalness;

        private static readonly string[] cardNamesToIgnore = new string[] { "Square Kompas Logo" };

        protected static readonly Dictionary<string, string> cardJsons = new Dictionary<string, string>();
        protected static readonly Dictionary<string, string> cardFileNames = new Dictionary<string, string>();
        private static IReadOnlyCollection<string> CardNames => cardJsons.Keys;

        protected static readonly Dictionary<string, string> keywordJsons = new Dictionary<string, string>();
        protected static readonly Dictionary<string, string> partialKeywordJsons = new Dictionary<string, string>();
        protected static readonly Dictionary<string, string> triggerKeywordJsons = new Dictionary<string, string>();

        public static ReminderTextsContainer Reminders { get; private set; }
        public static ICollection<string> Keywords { get; private set; }

        private static bool initalized = false;
        private static readonly object initializationLock = new object();

        public Sprite frameTexture;
        public Sprite namePlacardTexture;
        public Sprite typePlacardTexture;
        public Sprite effectTextTexture;
        public Sprite cardBackTexture;
        public Sprite nTexture;
        public Sprite eTexture;
        public Sprite sacTexture;
        public Sprite wTexture;
        public Sprite rTexture;
        public Sprite dTexture;

        public Game game;
        public Settings Settings
        {
            get
            {
                if (game != null) return game.Settings;
                else return default;
            }
        }

        public static IEnumerable<string> CardJsons => cardJsons.Values;

        public static void Init() => InitializeCardJsons();

        protected virtual void Awake()
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

                InitializeTextures();
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

        private void InitializeTextures()
        {
            Debug.Log("Initializing textures");
            var textures = new TextureParams.Textures()
            {
                NamePlacardTexture = namePlacardTexture,
                TypePlacardTexture = typePlacardTexture,
                EffectTextTexture = effectTextTexture,
                CharacterArt = LoadSprite(cardFileNames.Keys.FirstOrDefault()),
                CardBackTexture = cardBackTexture,
                NTexture = nTexture,
                ETexture = eTexture,
                SACTexture = sacTexture,
                WTexture = wTexture,
                RTexture = rTexture,
                DTexture = dTexture,
            };

            textures.FrameColorOverride = Settings?.FriendlyColor ?? Settings.DefaultFriendlyBlue;
            (friendlyZoomedCharTexture, friendlyZoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: true, textures), true);
            (friendlyZoomedNonCharTexture, friendlyZoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: false, textures), true);
            (friendlyUnzoomedCharTexture, friendlyUnzoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: true, textures), true);
            (friendlyUnzoomedNonCharTexture, friendlyUnzoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: false, textures), true);

            Debug.Log("Friendly textures initialized");

            textures.FrameColorOverride = Settings?.EnemyColor ?? Settings.DefaultEnemyRed;
            (enemyZoomedCharTexture, enemyZoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: true, textures), true);
            (enemyZoomedNonCharTexture, enemyZoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: true, isChar: false, textures), true);
            (enemyUnzoomedCharTexture, enemyUnzoomedCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: true, textures), true);
            (enemyUnzoomedNonCharTexture, enemyUnzoomedNonCharMetalness) = VoxelCard.BuildTexture(default, default, TextureParams.Params(isZoomed: false, isChar: false, textures), true);

            Debug.Log("enemy textures initialized");
        }

        public CardTextures GetTextures(string cardFileName, bool isChar, bool friendly)
        {
            CardTextures ret;
            if (cardFileNameToTextures.ContainsKey(cardFileName)) ret = cardFileNameToTextures[cardFileName];
            else
            {
                CardTextures baseline = DetermineBaselineTextures(isChar, friendly);

                var zoomedTexCharArt = AssetDatabase.LoadAssetAtPath(Path.Combine(ZoomedTextureFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
                var zoomedMetCharArt = AssetDatabase.LoadAssetAtPath(Path.Combine(ZoomedMetalnessFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
                var unzoomedTexCharArt = AssetDatabase.LoadAssetAtPath(Path.Combine(UnzoomedTextureFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;
                var unzoomedMetCharArt = AssetDatabase.LoadAssetAtPath(Path.Combine(UnzoomedMetalnessFolder, $"{cardFileName}.asset"), typeof(Texture2D)) as Texture2D;

                Debug.Log($"Loading fresh {Path.Combine(ZoomedTextureFolder, $"{cardFileName}.asset")}. Is it null? {zoomedTexCharArt == null}");

                var copiedZoomedTex = Copy(baseline.zoomedTex, zoomedTexCharArt);
                var copiedZoomedMet = Copy(baseline.zoomedMet, zoomedMetCharArt);
                var copiedUnzoomedTex = Copy(baseline.unzoomedTex, unzoomedTexCharArt);
                var copiedUnzoomedMet = Copy(baseline.unzoomedMet, unzoomedMetCharArt);
                ret = new CardTextures(copiedZoomedTex, copiedZoomedMet, copiedUnzoomedTex, copiedUnzoomedMet);
                cardFileNameToTextures[cardFileName] = ret;
            }

            return ret;
        }

        private static CardTextures DetermineBaselineTextures(bool isChar, bool friendly)
        {
            if (friendly) return isChar
                    ? new CardTextures(friendlyZoomedCharTexture, friendlyZoomedCharMetalness,
                        friendlyUnzoomedCharTexture, friendlyUnzoomedCharMetalness)
                    : new CardTextures(friendlyZoomedNonCharTexture, friendlyZoomedNonCharMetalness,
                        friendlyUnzoomedNonCharTexture, friendlyUnzoomedNonCharMetalness);
            else return isChar
                    ? new CardTextures(enemyZoomedCharTexture, enemyZoomedCharMetalness,
                        enemyUnzoomedCharTexture, enemyUnzoomedCharMetalness)
                    : new CardTextures(enemyZoomedNonCharTexture, enemyZoomedNonCharMetalness,
                        enemyUnzoomedNonCharTexture, enemyUnzoomedNonCharMetalness);
        }

        private Texture2D Copy(Texture2D baseTexture, Texture2D cardSpecificTexture)
        {
            var texture = new Texture2D(baseTexture.width, baseTexture.height);
            texture.SetPixels(baseTexture.GetPixels());
            var width = cardSpecificTexture.width;
            var height = cardSpecificTexture.height;
            texture.SetPixels(width, height - 1, width, height, cardSpecificTexture.GetPixels(0, 0, width, height));
            texture.Apply();
            return texture;
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

        public static string FileNameFor(string cardName) => cardFileNames[cardName];

        public static Sprite LoadSprite(string cardFileName) => Resources.Load<Sprite>(Path.Combine("Simple Sprites", cardFileName));
    }
}