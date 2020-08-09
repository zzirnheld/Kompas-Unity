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
using UnityEngine;

public class CardRepository : MonoBehaviour
{
    public const string cardListFilePath = "Card Jsons/Card List";
    public const string cardJsonsFolderpath = "Card Jsons/";

    private static Dictionary<string, string> cardJsons;
    private static Dictionary<string, int> cardNameIDs;
    private static List<string> cardNames;
    private static List<string> cardNamesToIgnore;

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
            Debug.Log($"Adding json for \"{nameClean}\" of length {nameClean.Length} to dictionary");
            cardJsons.Add(nameClean, json);
        }
    }

    private bool IsCardToIgnore(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return true;
        return cardNamesToIgnore.Contains(name);
    }

    public static string CardName(int id) => cardNames[id];
    public static int CardNameID(string cardName) => cardNameIDs[cardName];
    public static bool CardExists(string cardName) => cardNameIDs.ContainsKey(cardName);

    private bool SubtypesContain(string cardName, string subtypesInclude)
    {
        if (!cardJsons.ContainsKey(cardName)) return false;
        try
        {
            SerializableCard card = JsonUtility.FromJson<SerializableCard>(cardJsons[cardName]);
            if(card.subtypeText == null) return false;
            return ContainsIgnoreCase(card.subtypeText, subtypesInclude);
        }
        catch(System.ArgumentException)
        {
            Debug.LogError($"Arg ex when checking if subtypes of {cardName} contain {subtypesInclude}. Json is {cardJsons[cardName]}");
            return false;
        }
    }

    private bool ContainsIgnoreCase(string a, string b)
    {
        return a.ToLower().Contains(b.ToLower());
    }

    public List<string> GetCardsFromFilter(string nameIncludes, string subtypeIncludes)
    {
        List<string> cards = new List<string>();
        foreach (string name in cardNames)
        {
            if (ContainsIgnoreCase(name, nameIncludes) && SubtypesContain(name, subtypeIncludes))
            {
                //Debug.Log($"found a name {name} that contains {nameIncludes}");
                cards.Add(name);
            }
        }
        return cards;
    }

    public List<string> GetJsonsFromNameList(List<string> names)
    {
        List<string> jsons = new List<string>();
        foreach(string name in names)
        {
            //Debug.Log($"Trying to get json for name \"{name}\", string length {name.Length}");
            if(cardJsons.ContainsKey(name)) jsons.Add(cardJsons[name]);
        }
        return jsons;
    }

    public List<string> GetJsonsThatFit(string nameIncludes, string subtypesInclude)
    {
        return GetJsonsFromNameList(GetCardsFromFilter(nameIncludes, subtypesInclude));
    }

    public string GetJsonFromName(string name)
    {
        if (!cardJsons.ContainsKey(name))
        {
            Debug.LogError($"No json found for name \"{name ?? "null"}\" of length {name?.Length ?? 0}");
            return null;
        }

        return cardJsons[name];
    }

    #region Create Cards
    public SerializableCard GetCardFromName(string name)
    {
        if (!CardExists(name)) return null;

        return JsonUtility.FromJson<SerializableCard>(cardJsons[name]);
    }

    private ServerEffect[] CreateServerEffects(SerializableEffect[] serEffs, GameCard card, ServerGame serverGame, ServerPlayer owner)
    {
        ServerEffect[] effects = new ServerEffect[serEffs.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new ServerEffect(serEffs[i], card, serverGame, owner, i);
        }
        return effects;
    }

    public AvatarServerGameCard InstantiateServerAvatar(string cardName, ServerGame serverGame, ServerPlayer owner, int id)
    {
        if (!cardJsons.ContainsKey(cardName))
        {
            Debug.LogError($"Tried to create an avatar for a name that doesn't have a json");
            return null;
        }

        try
        {
            SerializableCard charCard = JsonUtility.FromJson<SerializableCard>(cardJsons[cardName]);
            AvatarServerGameCard avatar = Instantiate(ServerAvatarPrefab).GetComponent<AvatarServerGameCard>();
            ServerEffect[] effects = CreateServerEffects(charCard.effects, avatar, serverGame, owner);
            avatar.SetInfo(charCard, serverGame, owner, effects, id);
            avatar.cardCtrl.SetImage(avatar.CardName, false);
            serverGame.cardsByID.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message} \nJson was {cardJsons[cardName]}");
            return null;
        }
    }

    public ServerGameCard InstantiateServerNonAvatar(string name, ServerGame serverGame, ServerPlayer owner, int id)
    {
        Debug.Log($"Instantiating new server non avatar for name {name}");
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        ServerGameCard card = null;
        ServerEffect[] effects;

        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
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
            effects = CreateServerEffects(serializableCard.effects, card, serverGame, owner);
            card?.SetInfo(serializableCard, serverGame, owner, effects, id);
            card?.cardCtrl?.SetImage(card.CardName, false);
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    private ClientEffect[] CreateClientEffects(SerializableEffect[] serEffs, GameCard card, ClientGame clientGame)
    {
        ClientEffect[] effects = new ClientEffect[serEffs.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new ClientEffect(serEffs[i], card, clientGame, i);
        }
        return effects;
    }

    public AvatarClientGameCard InstantiateClientAvatar(string cardName, ClientGame clientGame, ClientPlayer owner, int id)
    {
        if (!cardJsons.ContainsKey(cardName))
        {
            Debug.LogError($"Tried to create an avatar for a name that doesn't have a json");
            return null;
        }

        try
        {
            SerializableCard charCard = JsonUtility.FromJson<SerializableCard>(cardJsons[cardName]);
            if (charCard.cardType != 'C') return null;
            AvatarClientGameCard avatar = Instantiate(ClientAvatarPrefab).GetComponent<AvatarClientGameCard>();
            ClientEffect[] effects = CreateClientEffects(charCard.effects, avatar, clientGame);
            avatar.SetInfo(charCard, clientGame, owner, effects, id);
            avatar.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
            avatar.cardCtrl.SetImage(avatar.CardName, false);
            clientGame.cardsByID.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message} \nJson was {cardJsons[cardName]}");
            return null;
        }
    }

    public ClientGameCard InstantiateClientNonAvatar(string name, ClientGame clientGame, ClientPlayer owner, int id)
    {
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
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
            ClientEffect[] effects = CreateClientEffects(serializableCard.effects, card, clientGame);
            card.SetInfo(serializableCard, clientGame, owner, effects, id);
            card.cardCtrl.SetImage(card.CardName, false);
            card.gameObject.GetComponentInChildren<ClientCardMouseController>().ClientGame = clientGame;
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
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
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    public DeckbuilderCard InstantiateDeckbuilderCard(string json, CardSearchController searchCtrl, Transform parent, bool inDeck)
    {
        try
        {
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCard serializableChar = JsonUtility.FromJson<SerializableCard>(json);
                    var charCard = Instantiate(DeckbuilderCharPrefab, parent).GetComponent<DeckbuilderCharCard>();
                    charCard.SetInfo(searchCtrl, serializableChar, inDeck);
                    return charCard;
                case 'S':
                    SerializableCard serializableSpell = JsonUtility.FromJson<SerializableCard>(json);
                    var spellCard = Instantiate(DeckbuilderSpellPrefab, parent).GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell, inDeck);
                    return spellCard;
                case 'A':
                    SerializableCard serializableAug = JsonUtility.FromJson<SerializableCard>(json);
                    var augCard = Instantiate(DeckbuilderAugPrefab, parent).GetComponent<DeckbuilderAugCard>();
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
    #endregion Create Cards
}
