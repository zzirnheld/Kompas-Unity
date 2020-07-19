using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class CardRestriction
{
    public Subeffect Subeffect { get; private set; }

    #region restrictions
    public const string NameIs = "Name Is"; // 1,
    public const string DistinctNameFromTargets = "Distinct Name from Other Targets"; // 11,
    public const string SameName = "Same Name as Target"; // 12,
    public const string DistinceNameFromSource = "Distinct Name from Source"; // 13,

    public const string SubtypesInclude = "Subtypes Include"; // 2,
    public const string SubtypesExclude = "Subtypes Exclude"; // 6,

    public const string IsCharacter = "Is Character"; //3,
    public const string IsSpell = "Is Spell"; //4,
    public const string IsAugment = "Is Augment"; // 5,
    public const string NotAugment = "Not Augment";

    public const string Friendly = "Friendly"; // 7,
    public const string Enemy = "Enemy"; //9,
    public const string SameOwner = "Same Owner as Source"; // 8,

    public const string Summoned = "Summoned"; // 10,
    public const string Avatar = "Avatar"; // 50,

    public const string Distinct = "Distinct from Source"; // 99,

    //location
    public const string Hand = "Hand"; // 100,
    public const string Discard = "Discard"; // 101,
    public const string Deck = "Deck"; // 102,
    public const string Board = "Board"; // 103,
    public const string LocationInList = "Multiple Possible Locations"; // 104,

    //stats
    public const string NLTEX = "N<=X"; // 200, //N <= X
    public const string ELTEX = "E<=X"; //201,
    public const string SLTEX = "S<=X"; //202,
    public const string WLTEX = "W<=X"; //203,
    public const string CostLTEX = "Cost<=X"; // 204,

    public const string NEX = "N==X"; //210, //N == X
    public const string EEX = "E==X"; //211,
    public const string SEX = "S==X"; //212,
    public const string WEX = "W==X"; //213,
    public const string CostEX = "Cost==X"; //214,

    public const string NLTX = "N<X"; //220,
    public const string ELTX = "E<X"; //221,
    public const string SLTX = "S<X"; //222,
    public const string WLTX = "W<X"; //223,

    public const string NLTEC = "N<=C"; //300, //N <= constant
    public const string ELTEC = "E<=C"; //302,
    public const string SLTEC = "S<=C"; //303,
    public const string WLTEC = "W<=C"; //304,

    //index in list
    public const string IndexInListGTEC = "Index>=C"; // 500,
    public const string IndexInListLTEC = "Index<=C"; //501,
    public const string IndexInListLTEX = "Index<=X"; //551,

    //misc
    public const string NotAlreadyTarget = "Not Already Target"; //600,
    public const string CanBePlayed = "Can Be Played"; // 601,
    public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost"; // 602

    //board
    public const string Adjacent = "Adjacent"; // 0,
    public const string WithinCSpaces = "Within C Spaces"; // 1,
    public const string InAOE = "In AOE"; // 2,
    public const string DistanceToTargetWithinCSpaces = "Target Within C Spaces of Source"; // 10,
    public const string AdjacentToSubtype = "Adjacent to Subtype"; // 20,
    public const string ExactlyXSpaces = "Exactly X Spaces to Source"; // 100,
    #endregion restrictions

    //because JsonUtility will fill in all values with defaults if not present
    public string[] cardRestrictions = new string[0];

    public int costsLessThan;
    public string nameIs;
    public string[] subtypesInclude = new string[0];
    public string[] subtypesExclude = new string[0];
    public int constant;
    public CardLocation[] locations;
    public int costMultiplier = 1;
    public int costDivisor = 1;

    public int cSpaces;
    public string[] adjacencySubtypes = new string[0];

    public string blurb = "";

    public void Initialize(Subeffect subeff)
    {
        this.Subeffect = subeff;
    }

    public virtual bool Evaluate (GameCard potentialTarget, int x)
    {
        if (potentialTarget == null) return false;

        foreach (string c in cardRestrictions)
        {
            //Debug.Log("Considering restriction " + c + " when X equals " + x);
            switch (c)
            {
                case NameIs:
                    if (potentialTarget.CardName != nameIs) return false;
                    break;
                case SubtypesInclude:
                    foreach (string s in subtypesInclude)
                    {
                        if (potentialTarget.SubtypeText.IndexOf(s) == -1) return false;
                    }
                    break;
                case IsCharacter:
                    if (potentialTarget.CardType != 'C') return false;
                    break;
                case IsSpell:
                    if (potentialTarget.CardType != 'S') return false;
                    break;
                case IsAugment:
                    if (potentialTarget.CardType != 'A') return false;
                    break;
                case NotAugment:
                    if (potentialTarget.CardType == 'A') return false;
                    break;
                case SubtypesExclude:
                    foreach (string s in subtypesExclude)
                    {
                        if (potentialTarget.SubtypeText.IndexOf(s) != -1) return false;
                    }
                    break;
                case Friendly:
                    if (potentialTarget.Controller != Subeffect.Controller) return false;
                    break;
                case Enemy:
                    if (potentialTarget.Controller == Subeffect.Controller) return false;
                    break;
                case SameOwner:
                    if (potentialTarget.Owner != Subeffect.Controller) return false;
                    break;
                case Summoned:
                    if (!potentialTarget.Summoned) return false;
                    break;
                case DistinctNameFromTargets:
                    //checks if any target shares a name with this one
                    if (Subeffect.Effect.Targets.Where(card => card.CardName == potentialTarget.CardName).Any()) return false;
                    break;
                case SameName: //note: same name as this subeffect's target
                    if (Subeffect.Target.CardName != potentialTarget.CardName) return false;
                    break;
                case DistinceNameFromSource:
                    if (Subeffect.Source.CardName == potentialTarget.CardName) return false;
                    break;
                case Avatar:
                    if (!(potentialTarget.IsAvatar)) return false;
                    break;
                case Distinct:
                    if (potentialTarget == Subeffect.Source) return false;
                    break;
                case Hand:
                    if (potentialTarget.Location != CardLocation.Hand) return false;
                    break;
                case Deck:
                    if (potentialTarget.Location != CardLocation.Deck) return false;
                    break;
                case Discard:
                    if (potentialTarget.Location != CardLocation.Discard) return false;
                    break;
                case Board:
                    if (potentialTarget.Location != CardLocation.Field) return false;
                    break;
                case LocationInList:
                    if (!locations.Contains(potentialTarget.Location)) return false;
                    break;
                case NLTEX:
                    if (potentialTarget.N > x) return false;
                    break;
                case ELTEX:
                    if (potentialTarget.E > x) return false;
                    break;
                case SLTEX:
                    if (potentialTarget.S > x) return false;
                    break;
                case WLTEX:
                    if (potentialTarget.W > x) return false;
                    break;
                case CostLTEX:
                    if (potentialTarget.Cost > x) return false;
                    break;
                case NEX:
                    if (potentialTarget.N != x) return false;
                    break;
                case EEX:
                    if (potentialTarget.E != x) return false;
                    break;
                case SEX:
                    if (potentialTarget.S != x) return false;
                    break;
                case WEX:
                    if (potentialTarget.W != x) return false;
                    break;
                case CostEX:
                    if (potentialTarget.Cost != x) return false;
                    break;
                case (NLTX):
                    if (potentialTarget.N >= x) return false;
                    break;
                case (ELTX):
                    if (potentialTarget.E >= x) return false;
                    break;
                case (SLTX):
                    if (potentialTarget.S >= x) return false;
                    break;
                case (WLTX):
                    if (potentialTarget.W >= x) return false;
                    break;
                case NLTEC:
                    if (potentialTarget.N > constant) return false;
                    break;
                case ELTEC:
                    if (potentialTarget.E > constant) return false;
                    break;
                case SLTEC:
                    if (potentialTarget.S > constant) return false;
                    break;
                case WLTEC:
                    if (potentialTarget.W > constant) return false;
                    break;
                case IndexInListGTEC:
                    if (potentialTarget.IndexInList < constant) return false;
                    break;
                case IndexInListLTEC:
                    if (potentialTarget.IndexInList > constant) return false;
                    break;
                case IndexInListLTEX:
                    if (potentialTarget.IndexInList > Subeffect.Effect.X) return false;
                    break;
                case NotAlreadyTarget:
                    if (Subeffect.Effect.Targets.Contains(potentialTarget)) return false;
                    break;
                case CanBePlayed:
                    bool found = false;
                    for(int i = 0; i < 7 && !found; i++)
                    {
                        for(int j = 0; j < 7 && !found; j++)
                        {
                            if (potentialTarget.PlayRestriction.EvaluateEffectPlay(i, j, Subeffect.Effect))
                            {
                                found = true;
                            }
                        }
                    }
                    if (found) break;
                    else return false;
                case EffectControllerCanPayCost:
                    if (Subeffect.Effect.Controller.Pips < potentialTarget.Cost * costMultiplier / costDivisor) return false;
                    break;
                case Adjacent:
                    if (!potentialTarget.IsAdjacentTo(Subeffect.Source)) return false;
                    break;
                case WithinCSpaces:
                    if (!potentialTarget.WithinSlots(cSpaces, Subeffect.Source)) return false;
                    break;
                case InAOE:
                    if (!Subeffect.Source.CardInAOE(potentialTarget)) return false;
                    break;
                case DistanceToTargetWithinCSpaces:
                    if (potentialTarget.DistanceTo(Subeffect.Source) > cSpaces) return false;
                    break;
                case AdjacentToSubtype:
                    if (!potentialTarget.AdjacentCards.Any(card => adjacencySubtypes.All(s => card.Subtypes.Contains(s)))) return false;
                    break;
                case ExactlyXSpaces:
                    if (potentialTarget.DistanceTo(Subeffect.Source) != Subeffect.Effect.X) return false;
                    break;
                default:
                    Debug.LogError($"You forgot to implement a check for {c}");
                    return false;
            }
        }

        Debug.Log(potentialTarget.CardName + " fits the restriction");
        return true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="potentialTarget"></param>
    /// <param name="actuallyTargetThis">Whether effect resolution should continue if this is a valid target</param>
    /// <returns></returns>
    public virtual bool Evaluate(GameCard potentialTarget)
    {
        return Evaluate(potentialTarget, Subeffect.Effect.X);
    }
}
