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

    public CardSearchController searchCtrl;

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
            //Debug.Log($"Loading json for name {nameClean}, path is {cardJsonsFolderpath + nameClean}");
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

    public SerializableCard GetCardFromName(string name)
    {
        if (!CardExists(name)) return null;

        return JsonUtility.FromJson<SerializableCard>(cardJsons[name]);
    }

    public AvatarCard InstantiateAvatar(string cardName, GameObject avatarPrefab, Game game, Player owner, int id)
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
            AvatarCard avatar = Instantiate(avatarPrefab).GetComponent<AvatarCard>();
            avatar.SetInfo(charCard, game, owner);
            avatar.SetImage(charCard.cardName);
            avatar.ID = id;
            game.cards.Add(id, avatar);
            return avatar;
        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {cardName} as Avatar, argument exception with message {argEx.Message} \nJson was {cardJsons[cardName]}");
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

    public DeckbuilderCard InstantiateDeckbuilderCard(string json, Transform parent, bool inDeck)
    {
        try
        {
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                    DeckbuilderCharCard charCard = Instantiate(searchCtrl.CharPrefab, parent)
                        .GetComponent<DeckbuilderCharCard>();
                    charCard.SetInfo(searchCtrl, serializableChar, inDeck);
                    return charCard;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    DeckbuilderSpellCard spellCard = Instantiate(searchCtrl.SpellPrefab, parent)
                        .GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell, inDeck);
                    return spellCard;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                    DeckbuilderAugCard augCard = Instantiate(searchCtrl.AugPrefab, parent)
                        .GetComponent<DeckbuilderAugCard>();
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
}
