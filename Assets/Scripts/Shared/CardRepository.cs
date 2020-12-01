using KompasClient.Cards;
using KompasClient.Effects;
using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.Effects;
using KompasDeckbuilder;
using KompasServer.Cards;
using KompasServer.Effects;
using KompasServer.GameCore;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;

public class CardRepository : MonoBehaviour
{
    public const string cardListFilePath = "Card Jsons/Card List";
    public const string cardJsonsFolderpath = "Card Jsons/";

    private static Dictionary<string, string> cardJsons;
    private static Dictionary<string, int> cardNameIDs;
    private static List<string> cardNames;
    private static List<string> cardNamesToIgnore;

    public static IEnumerable<string> CardJsons => cardJsons.Values;

    #region prefabs
    public GameObject DeckSelectCardPrefab;

    public GameObject DeckbuilderCharPrefab;
    public GameObject DeckbuilderSpellPrefab;
    public GameObject DeckbuilderAugPrefab;

    public GameObject ClientAvatarPrefab;
    public GameObject ClientCharPrefab;
    public GameObject ClientSpellPrefab;
    public GameObject ClientAugPrefab;

    public GameObject ServerAvatarPrefab;
    public GameObject ServerCharPrefab;
    public GameObject ServerSpellPrefab;
    public GameObject ServerAugPrefab;
    #endregion prefabs

    void Awake()
    {
        cardNamesToIgnore = new List<string>(new string[] {
            "Square Kompas Logo"
        });

        cardJsons = new Dictionary<string, string>();
        cardNameIDs = new Dictionary<string, int>();
        cardNames = new List<string>();
        string cardList = Resources.Load<TextAsset>(cardListFilePath).text;
        cardList = cardList.Replace('\r', '\n');
        string[] cardNameArray = cardList.Split('\n');

        foreach (string name in cardNameArray)
        {
            string nameClean = name.Substring(0, name.Length).Replace(":", "");
            //don't add duplicate cards
            if (IsCardToIgnore(nameClean) || CardExists(nameClean)) continue;
            //add the card's name to the list of card names
            cardNameIDs.Add(name, cardNames.Count);
            cardNames.Add(nameClean);

            //load the json
            string json = Resources.Load<TextAsset>(cardJsonsFolderpath + nameClean)?.text;
            if (json == null)
            {
                Debug.LogError($"Failed to load json for {nameClean}");
                continue;
            }
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");
            //add the cleaned json to the dictionary
            Debug.Log($"Adding json for \"{nameClean}\" of length {nameClean.Length} to dictionary. Json:\n{json}");
            cardJsons.Add(nameClean, json);
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

    #region Create Cards
    public SerializableCard GetCardFromName(string name)
    {
        if (!CardExists(name)) return null;

        return JsonUtility.FromJson<SerializableCard>(cardJsons[name]);
    }

    public AvatarServerGameCard InstantiateServerAvatar(string cardName, ServerGame serverGame, ServerPlayer owner, int id)
    {
        Debug.Log($"Instantiating server avatar named {cardName}");

        if (!cardJsons.ContainsKey(cardName))
        {
            Debug.LogError($"Tried to create an avatar for a name that doesn't have a json");
            return null;
        }



        try
        {
            Debug.Log("loading avatar json");
            ServerSerializableCard card = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[cardName],
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            Debug.Log("loading avatar gameobj");
            AvatarServerGameCard avatar = Instantiate(ServerAvatarPrefab).GetComponent<AvatarServerGameCard>();
            if (avatar == null) Debug.LogError("AVATAR WAS NULL");
            Debug.Log("setting avatar info");
            avatar.SetInfo(card, serverGame, owner, card.effects, id);
            Debug.Log("loading avatar img");
            avatar.cardCtrl.SetImage(avatar.CardName, false);
            Debug.Log("loading avatar id");
            serverGame.cardsByID.Add(id, avatar);
            Debug.Log("returning avatar");
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message}, stack trace:\n" +
                $"{argEx.StackTrace}\nJson was {cardJsons[cardName]}");
            return null;
        }
    }

    public ServerGameCard InstantiateServerNonAvatar(string name, ServerGame serverGame, ServerPlayer owner, int id)
    {
        Debug.Log($"Instantiating new server non avatar for name {name}");
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        ServerGameCard card = null;

        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            ServerSerializableCard serializableCard = JsonConvert.DeserializeObject<ServerSerializableCard>(cardJsons[name],
                new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            switch (serializableCard.cardType)
            {
                case 'C':
                    card = Instantiate(ServerCharPrefab).GetComponent<ServerGameCard>();
                    break;
                case 'S':
                    card = Instantiate(ServerSpellPrefab).GetComponent<ServerGameCard>();
                    break;
                case 'A':
                    card = Instantiate(ServerAugPrefab).GetComponent<ServerGameCard>();
                    break;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }
            card?.SetInfo(serializableCard, serverGame, owner, serializableCard.effects, id);
            card?.cardCtrl?.SetImage(card.CardName, false);
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    public AvatarClientGameCard InstantiateClientAvatar(string json, ClientGame clientGame, ClientPlayer owner, int id)
    {
        try
        {
            ClientSerializableCard charCard = JsonUtility.FromJson<ClientSerializableCard>(json);
            if (charCard.cardType != 'C') return null;
            AvatarClientGameCard avatar = Instantiate(ClientAvatarPrefab).GetComponent<AvatarClientGameCard>();
            avatar.SetInfo(charCard, clientGame, owner, charCard.effects, id);
            avatar.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
            avatar.cardCtrl.SetImage(avatar.CardName, false);
            clientGame.cardsByID.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load client Avatar, argument exception with message {argEx.Message},\n {argEx.StackTrace}, for json:\n{json}");
            return null;
        }
    }

    public ClientGameCard InstantiateClientNonAvatar(string json, ClientGame clientGame, ClientPlayer owner, int id)
    {
        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            ClientSerializableCard serializableCard = JsonUtility.FromJson<ClientSerializableCard>(json);
            ClientGameCard card;
            switch (serializableCard.cardType)
            {
                case 'C':
                    card = Instantiate(ClientCharPrefab).GetComponent<ClientGameCard>();
                    break;
                case 'S':
                    card = Instantiate(ClientSpellPrefab).GetComponent<ClientGameCard>();
                    break;
                case 'A':
                    card = Instantiate(ClientAugPrefab).GetComponent<ClientGameCard>();
                    break;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }
            Debug.Log($"Successfully created a card? {card != null} for json {json}");
            card.SetInfo(serializableCard, clientGame, owner, serializableCard.effects, id);
            card.cardCtrl.SetImage(card.CardName, false);
            card.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {json}, argument exception with message {argEx.Message}, {argEx.StackTrace}");
            return null;
        }
    }

    public DeckSelectCard InstantiateDeckSelectCard(string json, Transform parent, DeckSelectCard prefab, DeckSelectUIController uiCtrl)
    {
        try
        {
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
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

    public DeckbuilderCard InstantiateDeckbuilderCard(string json, CardSearchController searchCtrl, bool inDeck)
    {
        try
        {
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCard serializableChar = JsonUtility.FromJson<SerializableCard>(json);
                    var charCard = Instantiate(DeckbuilderCharPrefab).GetComponent<DeckbuilderCharCard>();
                    charCard.SetInfo(searchCtrl, serializableChar, inDeck);
                    return charCard;
                case 'S':
                    SerializableCard serializableSpell = JsonUtility.FromJson<SerializableCard>(json);
                    var spellCard = Instantiate(DeckbuilderSpellPrefab).GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell, inDeck);
                    return spellCard;
                case 'A':
                    SerializableCard serializableAug = JsonUtility.FromJson<SerializableCard>(json);
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
            return JsonUtility.FromJson<SerializableCard>(json);
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
