using KompasClient.Cards;
using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasDeckbuilder;
using KompasServer.Cards;
using KompasServer.Effects;
using KompasServer.GameCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRepository : MonoBehaviour
{
    public const string cardJsonsFolderPath = "Card Jsons/";
    public const string cardListFilePath = cardJsonsFolderPath + "Card List";

    public const string keywordJsonsFolderPath = "Keyword Jsons/Full Keywords/";
    public const string keywordListFilePath = keywordJsonsFolderPath + "Keyword List";

    public const string partialKeywordFolderPath = "Keyword Jsons/Partial Keywords/";
    public const string partialKeywordListFilePath = partialKeywordFolderPath + "Keyword List";

    private static readonly JsonSerializerSettings cardLoadingSettings = new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto,
        MaxDepth = null,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize
    };

    private static readonly string[] cardNamesToIgnore = new string[]
    {
        "Square Kompas Logo"
    };

    private static readonly Dictionary<string, string> cardJsons = new Dictionary<string, string>();
    private static readonly Dictionary<string, int> cardNameIDs = new Dictionary<string, int>();
    private static readonly List<string> cardNames = new List<string>();
    private static readonly Dictionary<string, string> keywordJsons = new Dictionary<string, string>();
    private static readonly Dictionary<string, string> partialKeywordJsons = new Dictionary<string, string>();
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

        string cardFilenameList = Resources.Load<TextAsset>(cardListFilePath).text;
        cardFilenameList = cardFilenameList.Replace('\r', '\n');
        string[] cardFilenameArray = cardFilenameList.Split('\n');

        foreach (string filename in cardFilenameArray)
        {
            //sanitize the filename. for some reason, doing substring fixes stuff
            string filenameClean = filename.Substring(0, filename.Length);
            //don't add duplicate cards
            if (IsCardToIgnore(filenameClean) || CardExists(filenameClean)) continue;

            //load the json
            var jsonAsset = Resources.Load<TextAsset>(cardJsonsFolderPath + filenameClean);
            if (jsonAsset == null)
            {
                Debug.LogError($"Failed to load json for {filenameClean}");
                continue;
            }
            string json = jsonAsset.text;
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");
            //load the cleaned json to get the card's name according to itself
            SerializableCard card = null;
            try
            {
                card = JsonConvert.DeserializeObject<SerializableCard>(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            }
            catch (JsonReaderException e)
            {
                Debug.LogError($"Failed to load {json}. Error\n{e}");
                continue;
            }
            string cardName = card.cardName;

            //add the cleaned json to the dictionary
            cardJsons.Add(cardName, json);
            //add the card's name to the list of card names
            cardNameIDs.Add(cardName, cardNames.Count);
            cardNames.Add(cardName);
        }

        string keywordList = Resources.Load<TextAsset>(keywordListFilePath).text;
        var keywords = keywordList.Replace('\r', '\n').Split('\n').Where(s => !string.IsNullOrEmpty(s));
        foreach (string keyword in keywords)
        {
            string json = Resources.Load<TextAsset>(keywordJsonsFolderPath + keyword).text;
            keywordJsons.Add(keyword, json);
        }

        string partialKeywordList = Resources.Load<TextAsset>(partialKeywordListFilePath).text;
        var partialKeywords = partialKeywordList.Replace('\r', '\n').Split('\n').Where(s => !string.IsNullOrEmpty(s));
        foreach (string keyword in partialKeywords)
        {
            string json = Resources.Load<TextAsset>(partialKeywordFolderPath + keyword).text;
            partialKeywordJsons.Add(keyword, json);
        }
    }

    private bool IsCardToIgnore(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return true;
        return cardNamesToIgnore.Contains(name);
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
        if (!partialKeywordJsons.ContainsKey(keyword)) return new ServerSubeffect[0];

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

    public AvatarServerGameCard InstantiateServerAvatar(string cardName, ServerGame serverGame, ServerPlayer owner, int id)
    {
        if (!cardJsons.ContainsKey(cardName)) return null;
        ServerSerializableCard card;
        List<ServerEffect> effects = new List<ServerEffect>();

        try
        {
            card = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[cardName], cardLoadingSettings);
            effects.AddRange(card.effects);
            for (int i = 0; i < card.keywords.Length; i++)
            {
                var s = card.keywords[i];
                var json = keywordJsons[s];
                var eff = JsonConvert.DeserializeObject<ServerEffect>(json, cardLoadingSettings);
                eff.arg = card.keywordArgs.Length > i ? card.keywordArgs[i] : 0;
                effects.Add(eff);
            }
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
        var cardComponents = avatarObj
            .GetComponents(typeof(GameCard))
            .Where(c => !(c is AvatarServerGameCard));
        foreach (var c in cardComponents) Destroy(c);

        var cardCtrlComponents = avatarObj
            .GetComponents(typeof(CardController))
            .Where(c => c is ClientCardController);
        foreach (var c in cardCtrlComponents) Destroy(c);

        var avatar = avatarObj.GetComponents<AvatarServerGameCard>().Where(c => c is AvatarServerGameCard).First();
        avatar.SetInitialCardInfo(card, serverGame, owner, effects.ToArray(), id);
        avatar.cardCtrl.SetImage(avatar.CardName, false);
        serverGame.cardsByID.Add(id, avatar);
        return avatar;
    }

    public ServerGameCard InstantiateServerNonAvatar(string name, ServerGame serverGame, ServerPlayer owner, int id)
    {
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        var cardObj = Instantiate(CardPrefab);
        ServerSerializableCard cardInfo;
        List<ServerEffect> effects = new List<ServerEffect>();

        try
        {
            cardInfo = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[name], cardLoadingSettings);
            effects.AddRange(cardInfo.effects);
            for (int i = 0; i < cardInfo.keywords.Length; i++)
            {
                var s = cardInfo.keywords[i];
                Debug.Log($"Trying to add keyword {s}");
                var keywordJson = keywordJsons[s];
                var eff = JsonConvert.DeserializeObject<ServerEffect>(keywordJson, cardLoadingSettings);
                eff.arg = cardInfo.keywordArgs.Length > i ? cardInfo.keywordArgs[i] : 0;
                effects.Add(eff);
            }
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, stacktrace {argEx.StackTrace}");
            return null;
        }

        //Destroy card components irrelevant to a server non-avatar
        var cardComponents = cardObj
            .GetComponents(typeof(GameCard))
            .Where(c => c is AvatarServerGameCard || !(c is ServerGameCard));
        foreach (var c in cardComponents) Destroy(c);

        var cardCtrlComponents = cardObj
            .GetComponents(typeof(CardController))
            .Where(c => c is ClientCardController);
        foreach (var c in cardCtrlComponents) Destroy(c);

        //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
        var card = cardObj.GetComponents<ServerGameCard>().Where(c => !(c is AvatarServerGameCard)).First();
        cardObj.GetComponents<CardController>().Where(c => !(c is ClientCardController)).First().card = card;

        card.SetInitialCardInfo(cardInfo, serverGame, owner, effects.ToArray(), id);
        card.cardCtrl.SetImage(card.CardName, false);
        return card;
    }

    public AvatarClientGameCard InstantiateClientAvatar(string json, ClientGame clientGame, ClientPlayer owner, int id)
    {
        ClientSerializableCard cardInfo;
        List<ClientEffect> effects = new List<ClientEffect>();

        try
        {
            cardInfo = JsonConvert.DeserializeObject<ClientSerializableCard>(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, MaxDepth = null, ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
            if (cardInfo.cardType != 'C') return null;
            effects.AddRange(cardInfo.effects);
            for (int i = 0; i < cardInfo.keywords.Length; i++)
            {
                var s = cardInfo.keywords[i];
                var keywordJson = keywordJsons[s];
                var eff = JsonConvert.DeserializeObject<ClientEffect>(keywordJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, MaxDepth = null, ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
                eff.arg = cardInfo.keywordArgs.Length > i ? cardInfo.keywordArgs[i] : 0;
                effects.Add(eff);
            }
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load client Avatar, argument exception with message {argEx.Message},\n {argEx.StackTrace}, for json:\n{json}");
            return null;
        }
        var avatarObj = Instantiate(CardPrefab);

        //Destroy card components irrelevant to a client avatar
        var cardComponents = avatarObj
            .GetComponents(typeof(GameCard))
            .Where(c => !(c is AvatarClientGameCard));
        foreach (var c in cardComponents) Destroy(c);

        var cardCtrlComponents = avatarObj
            .GetComponents(typeof(CardController))
            .Where(c => !(c is ClientCardController));
        foreach (var c in cardCtrlComponents) Destroy(c);

        //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
        var avatar = avatarObj.GetComponents<AvatarClientGameCard>().Where(c => c is AvatarClientGameCard).First();
        avatarObj.GetComponents<CardController>().Where(c => c is ClientCardController).First().card = avatar;

        avatar.SetInitialCardInfo(cardInfo, clientGame, owner, effects.ToArray(), id);
        avatar.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
        avatar.cardCtrl.SetImage(avatar.CardName, false);
        clientGame.cardsByID.Add(id, avatar);
        avatar.clientCardCtrl.ApplySettings(clientGame.clientUISettingsCtrl.ClientSettings);
        return avatar;
    }

    public ClientGameCard InstantiateClientNonAvatar(string json, ClientGame clientGame, ClientPlayer owner, int id)
    {
        ClientSerializableCard cardInfo;
        List<ClientEffect> effects = new List<ClientEffect>();

        try
        {
            cardInfo = JsonConvert.DeserializeObject<ClientSerializableCard>(json,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, MaxDepth = null, ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
            effects.AddRange(cardInfo.effects);
            for (int i = 0; i < cardInfo.keywords.Length; i++)
            {
                var s = cardInfo.keywords[i];
                var keywordJson = keywordJsons[s];
                var eff = JsonConvert.DeserializeObject<ClientEffect>(keywordJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto, ReferenceLoopHandling = ReferenceLoopHandling.Serialize });
                eff.arg = cardInfo.keywordArgs.Length > i ? cardInfo.keywordArgs[i] : 0;
                effects.Add(eff);
            }
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, {argEx.StackTrace}");
            return null;
        }
        var cardObj = Instantiate(CardPrefab);

        //Destroy card components irrelevant to a client non-avatar
        var cardComponents = cardObj
            .GetComponents(typeof(GameCard))
            .Where(c => c is AvatarClientGameCard || !(c is ClientGameCard));
        foreach (var c in cardComponents) Destroy(c);

        var cardCtrlComponents = cardObj
            .GetComponents(typeof(CardController))
            .Where(c => !(c is ClientCardController));
        foreach (var c in cardCtrlComponents) Destroy(c);

        //if don't use .where .first it still grabs components that should be destroyed, and are destroyed as far as i can tell
        var card = cardObj.GetComponents<ClientGameCard>().Where(c => !(c is AvatarClientGameCard)).First();
        var ctrl = cardObj.GetComponents<ClientCardController>().Where(c => c is ClientCardController).First();
        ctrl.card = card;
        ctrl.mouseCtrl.Card = card;

        Debug.Log($"Successfully created a card? {card != null} for json {json}");
        card.SetInitialCardInfo(cardInfo, clientGame, owner, effects.ToArray(), id);
        card.cardCtrl.SetImage(card.CardName, false);
        card.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
        card.clientCardCtrl.ApplySettings(clientGame.clientUISettingsCtrl.ClientSettings);
        return card;
    }

    public DeckSelectCard InstantiateDeckSelectCard(string json, Transform parent, DeckSelectCard prefab, DeckSelectUIController uiCtrl)
    {
        try
        {
            SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json);
            DeckSelectCard card = Instantiate(prefab, parent);
            card.SetInfo(serializableCard, uiCtrl);
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    public DeckbuilderCard InstantiateDeckbuilderCard(string json, DeckbuildSearchController searchCtrl, bool inDeck)
    {
        try
        {
            SerializableCard serializableCard = JsonConvert.DeserializeObject<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCard serializableChar = JsonConvert.DeserializeObject<SerializableCard>(json);
                    var charCard = Instantiate(DeckbuilderCharPrefab).GetComponent<DeckbuilderCharCard>();
                    charCard.SetInfo(searchCtrl, serializableChar, inDeck);
                    return charCard;
                case 'S':
                    SerializableCard serializableSpell = JsonConvert.DeserializeObject<SerializableCard>(json);
                    var spellCard = Instantiate(DeckbuilderSpellPrefab).GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell, inDeck);
                    return spellCard;
                case 'A':
                    SerializableCard serializableAug = JsonConvert.DeserializeObject<SerializableCard>(json);
                    var augCard = Instantiate(DeckbuilderAugPrefab).GetComponent<DeckbuilderAugCard>();
                    augCard.SetInfo(searchCtrl, serializableAug, inDeck);
                    return augCard;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    public static SerializableCard SerializableCardFromJson(string json)
    {
        try
        {
            return JsonConvert.DeserializeObject<SerializableCard>(json);
        }
        catch (System.ArgumentException e)
        {
            Debug.LogError($"{json} had argument exception {e.Message}");
        }
        return null;
    }

    public static IEnumerable<SerializableCard> GetSerializableCards(IEnumerable<string> jsons)
        => jsons.Select(json => SerializableCardFromJson(json)).Where(card => card != null);

    public static IEnumerable<SerializableCard> SerializableCards => GetSerializableCards(CardJsons);
    #endregion Create Cards
}
