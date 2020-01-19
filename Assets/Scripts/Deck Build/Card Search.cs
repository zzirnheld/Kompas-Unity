﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardSearch : MonoBehaviour
{
    public const string cardListFilePath = "Card Jsons/Card List";
    public const string cardJsonsFolderpath = "Card Jsons/";

    public TMP_Text CardSearchName;
    public GameObject CharPrefab;
    public GameObject SpellPrefab;
    public GameObject AugPrefab;

    private Dictionary<string, string> cardJsons;
    private List<string> cardNames;

    void Awake()
    {
        cardJsons = new Dictionary<string, string>();
        string cardList = Resources.Load<TextAsset>(cardListFilePath).text;
        cardList = cardList.Replace('\r', '\n');
        string[] cardNameArray = cardList.Split('\n');

        foreach (string name in cardNameArray)
        {
            string nameClean = name.Substring(0, name.Length);
            //don't add duplicate cards
            if (CardExists(nameClean)) continue;
            //add the card's name to the list of card names
            cardNames.Add(nameClean);

            //load the json
            string json = Resources.Load<TextAsset>(cardJsonsFolderpath + nameClean).text;
            //remove problematic chars for from json function
            json = json.Replace('\n', ' ');
            json = json.Replace("\r", "");
            json = json.Replace("\t", "");
            //add the cleaned json to the dictionary
            cardJsons.Add(nameClean, json);
        }
    }

    private bool CardExists(string cardName)
    {
        return cardNames.Contains(cardName);
    }
    public DeckbuilderCard InstantiateCard(string json)
    {
        SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
        switch (serializableCard.cardType)
        {
            case 'C':
                SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                DeckbuilderCharCard charCard = Instantiate(CharPrefab).GetComponent<DeckbuilderCharCard>();
                charCard.SetInfo(serializableChar);
                return charCard;
            case 'S':
                SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                DeckbuilderSpellCard spellCard = Instantiate(SpellPrefab).GetComponent<DeckbuilderSpellCard>();
                spellCard.SetInfo(serializableSpell);
                return spellCard;
            case 'A':
                SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                DeckbuilderAugCard augCard = Instantiate(AugPrefab).GetComponent<DeckbuilderAugCard>();
                augCard.SetInfo(serializableAug);
                return augCard;
            default:
                Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                return null;
        }
    }
    private List<DeckbuilderCard> SearchCards()
    {
        List<DeckbuilderCard> list = new List<DeckbuilderCard>();

        //for now, only search by name
        string cardNameToSearchFor = CardSearchName.text;
        foreach(string name in cardNames)
        {
            if (name.Contains(cardNameToSearchFor)){
                list.Add(InstantiateCard(cardJsons[name]));
            }
        }

        return list;
    }
}
