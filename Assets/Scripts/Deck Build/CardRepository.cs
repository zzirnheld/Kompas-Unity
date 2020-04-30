using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardRepository : MonoBehaviour
{
    public const string cardListFilePath = "Card Jsons/Card List";
    public const string cardJsonsFolderpath = "Card Jsons/";

    private Dictionary<string, string> cardJsons;
    private List<string> cardNames;
    private List<string> cardNamesToIgnore;

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

    public bool CardExists(string cardName)
    {
        return cardNames.Contains(cardName);
    }

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

    private ServerEffect[] CreateServerEffects(SerializableEffect[] serEffs, Card card)
    {
        ServerEffect[] effects = new ServerEffect[serEffs.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new ServerEffect(serEffs[i], card);
        }
        return effects;
    }

    public AvatarCard InstantiateServerAvatar(string cardName, ServerGame serverGame, Player owner, int id)
    {
        if (!cardJsons.ContainsKey(cardName))
        {
            Debug.LogError($"Tried to create an avatar for a name that doesn't have a json");
            return null;
        }

        try
        {
            SerializableCharCard charCard = JsonUtility.FromJson<SerializableCharCard>(cardJsons[cardName]);
            if (charCard.cardType != 'C') return null;
            AvatarCard avatar = Instantiate(ServerAvatarPrefab).GetComponent<AvatarCard>();
            ServerEffect[] effects = CreateServerEffects(charCard.effects, avatar);
            avatar.SetInfo(charCard, serverGame, owner, effects);
            avatar.SetImage();
            avatar.ID = id;
            serverGame.cards.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message} \nJson was {cardJsons[cardName]}");
            return null;
        }
    }

    public Card InstantiateServerNonAvatar(string name, ServerGame serverGame, Player owner, int id)
    {
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        Card card = null;
        Effect[] effects;

        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                    card = Instantiate(ServerCharPrefab).GetComponent<CharacterCard>();
                    effects = CreateServerEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableChar, serverGame, owner, effects);
                    break;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    card = Instantiate(ServerSpellPrefab).GetComponent<SpellCard>();
                    effects = CreateServerEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableSpell, serverGame, owner, effects);
                    break;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                    card = Instantiate(ServerSpellPrefab).GetComponent<AugmentCard>();
                    effects = CreateServerEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableAug, serverGame, owner, effects);
                    break;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }
            card?.SetImage();
            return card;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
            return null;
        }
    }

    private ClientEffect[] CreateClientEffects(SerializableEffect[] serEffs, Card card)
    {
        ClientEffect[] effects = new ClientEffect[serEffs.Length];
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = new ClientEffect(serEffs[i], card);
        }
        return effects;
    }

    public AvatarCard InstantiateClientAvatar(string cardName, ClientGame clientGame, Player owner, int id)
    {
        if (!cardJsons.ContainsKey(cardName))
        {
            Debug.LogError($"Tried to create an avatar for a name that doesn't have a json");
            return null;
        }

        try
        {
            SerializableCharCard charCard = JsonUtility.FromJson<SerializableCharCard>(cardJsons[cardName]);
            if (charCard.cardType != 'C') return null;
            AvatarCard avatar = Instantiate(ClientAvatarPrefab).GetComponent<AvatarCard>();
            ClientEffect[] effects = CreateClientEffects(charCard.effects, avatar);
            avatar.SetInfo(charCard, clientGame, owner, effects);
            avatar.gameObject.GetComponent<ClientCardMouseController>().ClientGame = clientGame;
            avatar.SetImage();
            avatar.ID = id;
            clientGame.cards.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message} \nJson was {cardJsons[cardName]}");
            return null;
        }
    }

    public Card InstantiateClientNonAvatar(string name, ClientGame clientGame, Player owner, int id)
    {
        string json = cardJsons[name] ?? throw new System.ArgumentException($"Name {name} not associated with json");
        Card card = null;
        Effect[] effects;

        try
        {
            //TODO later try setting serializableCard in the switch, and moving set info outside
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                    card = Instantiate(ClientCharPrefab).GetComponent<CharacterCard>();
                    effects = CreateClientEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableChar, clientGame, owner, effects);
                    break;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    card = Instantiate(ClientSpellPrefab).GetComponent<SpellCard>();
                    effects = CreateClientEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableSpell, clientGame, owner, effects);
                    break;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                    card = Instantiate(ClientAugPrefab).GetComponent<AugmentCard>();
                    effects = CreateClientEffects(serializableCard.effects, card);
                    card?.SetInfo(serializableAug, clientGame, owner, effects);
                    break;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }
            card?.SetImage();
            card.gameObject.GetComponent<ClientCardMouseController>().ClientGame = clientGame;
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
                    SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                    var charCard = Instantiate(DeckbuilderCharPrefab, parent).GetComponent<DeckbuilderCharCard>();
                    charCard.SetInfo(searchCtrl, serializableChar, inDeck);
                    return charCard;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    var spellCard = Instantiate(DeckbuilderSpellPrefab, parent).GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell, inDeck);
                    return spellCard;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
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
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
            return null;
        }
    }
    #endregion Create Cards
}
