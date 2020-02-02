using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardRespository : MonoBehaviour
{
    public const string cardListFilePath = "Card Jsons/Card List";
    public const string cardJsonsFolderpath = "Card Jsons/";

    private Dictionary<string, string> cardJsons;
    private List<string> cardNames;

    public CardSearchController searchCtrl;

    void Awake()
    {
        cardJsons = new Dictionary<string, string>();
        cardNames = new List<string>();
        string cardList = Resources.Load<TextAsset>(cardListFilePath).text;
        cardList = cardList.Replace('\r', '\n');
        string[] cardNameArray = cardList.Split('\n');

        foreach (string name in cardNameArray)
        {
            string nameClean = name.Substring(0, name.Length);
            //don't add duplicate cards
            if (string.IsNullOrWhiteSpace(nameClean) || CardExists(nameClean)) continue;
            //add the card's name to the list of card names
            cardNames.Add(nameClean);

            //load the json
            Debug.Log($"Loading json for name {nameClean}, path is {cardJsonsFolderpath + nameClean}");
            string json = Resources.Load<TextAsset>(cardJsonsFolderpath + nameClean)?.text;
            if (json == null) continue;
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");
            //add the cleaned json to the dictionary
            cardJsons.Add(nameClean, json);
        }
    }

    public bool CardExists(string cardName)
    {
        return cardNames.Contains(cardName);
    }

    public DeckbuilderCard InstantiateCard(string json, Transform parent)
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
                    charCard.SetInfo(searchCtrl, serializableChar);
                    return charCard;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    DeckbuilderSpellCard spellCard = Instantiate(searchCtrl.SpellPrefab, parent)
                        .GetComponent<DeckbuilderSpellCard>();
                    spellCard.SetInfo(searchCtrl, serializableSpell);
                    return spellCard;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                    DeckbuilderAugCard augCard = Instantiate(searchCtrl.AugPrefab, parent)
                        .GetComponent<DeckbuilderAugCard>();
                    augCard.SetInfo(searchCtrl, serializableAug);
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

    public List<string> GetCardsFromFilter(string nameIncludes)
    {
        List<string> cards = new List<string>();
        foreach (string name in cardNames)
        {
            if (name.Contains(nameIncludes))
            {
                Debug.Log($"found a name {name} that contains {nameIncludes}");
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
            Debug.Log($"Trying to get json for name \"{name}\"");
            if(cardJsons.ContainsKey(name)) jsons.Add(cardJsons[name]);
        }
        return jsons;
    }

    public List<string> GetJsonsThatFit(string nameIncludes)
    {
        return GetJsonsFromNameList(GetCardsFromFilter(nameIncludes));
    }
}
