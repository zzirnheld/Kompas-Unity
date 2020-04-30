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

    #region Subeffect Factories
    public static ServerSubeffect FromJson(SubeffectType seType, string subeffJson, Effect parent, int subeffIndex)
    {
        Debug.Log("Creating subeffect from json of type " + seType + " json " + subeffJson);

        Subeffect toReturn = null;

        switch (seType)
        {
            case SubeffectType.BoardTarget:
                toReturn = JsonUtility.FromJson<BoardTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.DeckTarget:
                toReturn = JsonUtility.FromJson<DeckTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.DiscardTarget:
                toReturn = JsonUtility.FromJson<DiscardTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.HandTarget:
                toReturn = JsonUtility.FromJson<HandTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetThis:
                toReturn = JsonUtility.FromJson<TargetThisSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetThisSpace:
                toReturn = JsonUtility.FromJson<TargetThisSpaceSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetAugmentedCard:
                toReturn = JsonUtility.FromJson<TargetAugmentedCardSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseFromList:
                toReturn = JsonUtility.FromJson<ChooseFromListSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseFromListSaveRest:
                toReturn = JsonUtility.FromJson<ChooseFromListSaveRestSubeffect>(subeffJson);
                break;
            case SubeffectType.DeleteTargetFromList:
                toReturn = JsonUtility.FromJson<DeleteTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.TargetAll:
                toReturn = JsonUtility.FromJson<TargetAllSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeNESW:
                toReturn = JsonUtility.FromJson<ChangeNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.XChangeNESW:
                toReturn = JsonUtility.FromJson<XChangeNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.AddPips:
                toReturn = JsonUtility.FromJson<AddPipsSubeffect>(subeffJson);
                break;
            case SubeffectType.Negate:
                toReturn = JsonUtility.FromJson<NegateSubeffect>(subeffJson);
                break;
            case SubeffectType.Dispel:
                toReturn = JsonUtility.FromJson<DispelSubeffect>(subeffJson);
                break;
            case SubeffectType.SwapOwnNESW:
                toReturn = JsonUtility.FromJson<SwapOwnNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeSpellC:
                toReturn = JsonUtility.FromJson<ChangeSpellCSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeAllNESW:
                toReturn = JsonUtility.FromJson<ChangeAllNESWSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByBoardCount:
                toReturn = JsonUtility.FromJson<SetXBoardRestrictionSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByGamestateValue:
                toReturn = JsonUtility.FromJson<SetXByGamestateSubeffect>(subeffJson);
                break;
            case SubeffectType.ChangeXByGamestateValue:
                toReturn = JsonUtility.FromJson<ChangeXByGamestateSubeffect>(subeffJson);
                break;
            case SubeffectType.PlayerChooseX:
                toReturn = JsonUtility.FromJson<PlayerChooseXSubeffect>(subeffJson);
                break;
            case SubeffectType.SpaceTarget:
                toReturn = JsonUtility.FromJson<SpaceTargetSubeffect>(subeffJson);
                break;
            case SubeffectType.PayPips:
                toReturn = JsonUtility.FromJson<PayPipsSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByTargetS:
                toReturn = JsonUtility.FromJson<SetXTargetSSubeffect>(subeffJson);
                break;
            case SubeffectType.SetXByTargetCost:
                toReturn = JsonUtility.FromJson<SetXByTargetCostSubeffect>(subeffJson);
                break;
            case SubeffectType.PlayCard:
                toReturn = JsonUtility.FromJson<PlaySubeffect>(subeffJson);
                break;
            case SubeffectType.PayPipsByTargetCost:
                toReturn = JsonUtility.FromJson<PayPipsTargetCostSubeffect>(subeffJson);
                break;
            case SubeffectType.DiscardCard:
                toReturn = JsonUtility.FromJson<DiscardSubeffect>(subeffJson);
                break;
            case SubeffectType.ReshuffleCard:
                toReturn = JsonUtility.FromJson<ReshuffleSubeffect>(subeffJson);
                break;
            case SubeffectType.RehandCard:
                toReturn = JsonUtility.FromJson<RehandSubeffect>(subeffJson);
                break;
            case SubeffectType.Draw:
                toReturn = JsonUtility.FromJson<DrawSubeffect>(subeffJson);
                break;
            case SubeffectType.DrawX:
                toReturn = JsonUtility.FromJson<DrawXSubeffect>(subeffJson);
                break;
            case SubeffectType.Bottomdeck:
                toReturn = JsonUtility.FromJson<BottomdeckSubeffect>(subeffJson);
                break;
            case SubeffectType.Topdeck:
                toReturn = JsonUtility.FromJson<TopdeckSubeffect>(subeffJson);
                break;
            case SubeffectType.Move:
                toReturn = JsonUtility.FromJson<MoveSubeffect>(subeffJson);
                break;
            case SubeffectType.XTimesLoop:
                toReturn = JsonUtility.FromJson<XTimesSubeffect>(subeffJson);
                break;
            case SubeffectType.TTimesLoop:
                toReturn = JsonUtility.FromJson<TTimesSubeffect>(subeffJson);
                break;
            case SubeffectType.WhileHaveTargetsLoop:
                toReturn = JsonUtility.FromJson<LoopWhileHaveTargetsSubeffect>(subeffJson);
                break;
            case SubeffectType.ExitLoopIfEffectImpossible:
                toReturn = JsonUtility.FromJson<ExitLoopIfEffectImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.JumpOnImpossible:
                toReturn = JsonUtility.FromJson<SkipToEffectOnImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.ClearOnImpossible:
                toReturn = JsonUtility.FromJson<ClearOnImpossibleSubeffect>(subeffJson);
                break;
            case SubeffectType.ChooseEffectOption:
                toReturn = JsonUtility.FromJson<ChooseOptionSubeffect>(subeffJson);
                break;
            case SubeffectType.EndEffect:
                toReturn = JsonUtility.FromJson<EndResolutionSubeffect>(subeffJson);
                break;
            case SubeffectType.CountXLoop:
                toReturn = JsonUtility.FromJson<CountXLoopSubeffect>(subeffJson);
                break;
            case SubeffectType.ConditionalEndEffect:
                toReturn = JsonUtility.FromJson<ConditionalEndSubeffect>(subeffJson);
                break;
            case SubeffectType.HangingNESWBuff:
                toReturn = JsonUtility.FromJson<TemporaryNESWBuffSubeffect>(subeffJson);
                break;
            case SubeffectType.DelaySubeffect:
                toReturn = JsonUtility.FromJson<DelaySubeffect>(subeffJson);
                break;
            case SubeffectType.HangingNESWBuffAll:
                toReturn = JsonUtility.FromJson<TemporaryNESWBuffAllSubeffect>(subeffJson);
                break;
            default:
                Debug.LogError($"Unrecognized effect type enum {seType} for loading effect in effect constructor");
                return null;
        }

        if (toReturn != null)
        {
            Debug.Log($"Finishing setup for new effect of type {seType}");
            toReturn.Initialize(parent, subeffIndex);
        }

        return toReturn;
    }
    #endregion Subeffect Factories

    #region Create Cards
    public SerializableCard GetCardFromName(string name)
    {
        if (!CardExists(name)) return null;

        return JsonUtility.FromJson<SerializableCard>(cardJsons[name]);
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
            avatar.SetInfo(charCard, serverGame, owner);
            avatar.SetImage(charCard.cardName);
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

        try
        {
            SerializableCard serializableCard = JsonUtility.FromJson<SerializableCard>(json);
            switch (serializableCard.cardType)
            {
                case 'C':
                    SerializableCharCard serializableChar = JsonUtility.FromJson<SerializableCharCard>(json);
                    card = Instantiate(ServerCharPrefab).GetComponent<CharacterCard>();
                    break;
                case 'S':
                    SerializableSpellCard serializableSpell = JsonUtility.FromJson<SerializableSpellCard>(json);
                    card = Instantiate(ServerSpellPrefab).GetComponent<SpellCard>();
                    break;
                case 'A':
                    SerializableAugCard serializableAug = JsonUtility.FromJson<SerializableAugCard>(json);
                    card = Instantiate(ServerSpellPrefab).GetComponent<AugmentCard>();
                    break;
                default:
                    Debug.LogError("Unrecognized type character " + serializableCard.cardType + " in " + json);
                    return null;
            }

            ServerEffect[] effects = new ServerEffect[serializableCard.effects.Length];

        }
        catch (System.ArgumentException argEx)
        {
            //Catch JSON parse error
            Debug.LogError($"Failed to load {name}, argument exception with message {argEx.Message}");
            return null;
        }

        card?.SetInfo(blabla);
        return card;
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
