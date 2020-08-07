using System;
using System.Linq;
using UnityEngine;
using KompasCore.Cards;

namespace KompasCore.Effects
{
    [Serializable]
    public class CardRestriction
    {
        public Subeffect Subeffect { get; private set; }

        #region restrictions
        //targets
        public const string AlreadyTarget = "Already Target";
        public const string NotAlreadyTarget = "Not Already Target";

        //names
        public const string NameIs = "Name Is";
        public const string SameName = "Same Name as Target";
        public const string DistinctNameFromTargets = "Distinct Name from Other Targets";
        public const string DistinctNameFromSource = "Distinct Name from Source";

        //card types
        public const string IsCharacter = "Is Character";
        public const string IsSpell = "Is Spell";
        public const string IsAugment = "Is Augment";
        public const string NotAugment = "Not Augment";

        //control
        public const string Friendly = "Friendly";
        public const string Enemy = "Enemy";
        public const string SameOwner = "Same Owner as Source";

        //summoned
        public const string Summoned = "Summoned";
        public const string Avatar = "Avatar";

        //subtypes
        public const string SubtypesInclude = "Subtypes Include";
        public const string SubtypesExclude = "Subtypes Exclude";

        //distinct
        public const string DistinctFromSource = "Distinct from Source";
        public const string DistinctFromTarget = "Distinct from Target";

        //location
        public const string Hand = "Hand";
        public const string Discard = "Discard";
        public const string Deck = "Deck";
        public const string Board = "Board";
        public const string LocationInList = "Multiple Possible Locations";

        //stats
            //<=X
        public const string NLTEX = "N<=X";
        public const string ELTEX = "E<=X";
        public const string SLTEX = "S<=X";
        public const string WLTEX = "W<=X";
        public const string CostLTEX = "Cost<=X";
            //==X
        public const string NEX = "N==X";
        public const string EEX = "E==X";
        public const string SEX = "S==X";
        public const string WEX = "W==X";
        public const string CostEX = "Cost==X";
            //<X
        public const string NLTX = "N<X";
        public const string ELTX = "E<X";
        public const string SLTX = "S<X";
        public const string WLTX = "W<X";
            //<=C
        public const string NLTEC = "N<=C";
        public const string ELTEC = "E<=C";
        public const string SLTEC = "S<=C";
        public const string WLTEC = "W<=C";

        //positioning
        public const string Adjacent = "Adjacent";
        public const string WithinCSpacesOfSource = "Within C Spaces";
        public const string InAOE = "In AOE";
        public const string AdjacentToSubtype = "Adjacent to Subtype";
        public const string ExactlyXSpaces = "Exactly X Spaces to Source";
        public const string InFrontOfSource = "In Front of Source";
        public const string IndexInListGTEC = "Index>=C";
        public const string IndexInListLTEC = "Index<=C";
        public const string IndexInListLTEX = "Index<=X";

        //misc
        public const string CanBePlayed = "Can Be Played";
        public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost";
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

        /// <summary>
        /// Determines whether the given restriction is true for a given card, with a given value of x.
        /// </summary>
        /// <param name="restriction">The restriction to check, as defined in the constants above.</param>
        /// <param name="potentialTarget">The card to consider the restriction for.</param>
        /// <param name="x">The value of x for which to consider the restriction</param>
        /// <returns><see langword="true"/> if the card fits the restriction for the given value of x, <see langword="false"/> otherwise.</returns>
        private bool RestrictionValid(string restriction, GameCard potentialTarget, int x)
        {
            Debug.Log($"Considering restriction {restriction} for card {potentialTarget} when X equals {x}");
            switch (restriction)
            {
                //targets
                case AlreadyTarget:    return Subeffect.Effect.Targets.Contains(potentialTarget);
                case NotAlreadyTarget: return !Subeffect.Effect.Targets.Contains(potentialTarget);

                //names
                case NameIs:                  return potentialTarget.CardName == nameIs;
                case SameName:                return Subeffect.Target.CardName == potentialTarget.CardName;
                case DistinctNameFromTargets: return Subeffect.Effect.Targets.All(card => card.CardName != potentialTarget.CardName);
                case DistinctNameFromSource:  return Subeffect.Source.CardName != potentialTarget.CardName;

                //card types
                case IsCharacter: return potentialTarget.CardType == 'C';
                case IsSpell:     return potentialTarget.CardType == 'S';
                case IsAugment:   return potentialTarget.CardType == 'A';
                case NotAugment:  return potentialTarget.CardType != 'A';

                //control
                case Friendly:  return potentialTarget.Controller == Subeffect.Controller;
                case Enemy:     return potentialTarget.Controller != Subeffect.Controller;
                case SameOwner: return potentialTarget.Owner == Subeffect.Controller;

                //summoned
                case Summoned: return potentialTarget.Summoned;
                case Avatar:   return potentialTarget.IsAvatar;

                //subtypes
                case SubtypesInclude: return subtypesInclude.All(s => potentialTarget.SubtypeText.Contains(s));
                case SubtypesExclude: return subtypesExclude.All(s => !potentialTarget.SubtypeText.Contains(s));

                //distinct
                case DistinctFromSource: return potentialTarget != Subeffect.Source;
                case DistinctFromTarget: return potentialTarget != Subeffect.Target;

                //location
                case Hand:           return potentialTarget.Location == CardLocation.Hand;
                case Deck:           return potentialTarget.Location == CardLocation.Deck;
                case Discard:        return potentialTarget.Location == CardLocation.Discard;
                case Board:          return potentialTarget.Location == CardLocation.Field;
                case LocationInList: return locations.Contains(potentialTarget.Location);

                //stats
                    //<=
                case NLTEX:    return potentialTarget.N <= x;
                case ELTEX:    return potentialTarget.E <= x;
                case SLTEX:    return potentialTarget.S <= x;
                case WLTEX:    return potentialTarget.W <= x;
                case CostLTEX: return potentialTarget.Cost <= x;
                    //==
                case NEX:      return potentialTarget.N == x;
                case EEX:      return potentialTarget.E == x;
                case SEX:      return potentialTarget.S == x;
                case WEX:      return potentialTarget.W == x;
                case CostEX:   return potentialTarget.Cost == x;
                    //<
                case NLTX:     return potentialTarget.N < x;
                case ELTX:     return potentialTarget.E < x;
                case SLTX:     return potentialTarget.S < x;
                case WLTX:     return potentialTarget.W < x;
                    //<=C
                case NLTEC:    return potentialTarget.N <= constant;
                case ELTEC:    return potentialTarget.E <= constant;
                case SLTEC:    return potentialTarget.S <= constant;
                case WLTEC:    return potentialTarget.W <= constant;

                //positioning
                case Adjacent:          return potentialTarget.IsAdjacentTo(Subeffect.Source);
                case AdjacentToSubtype: return potentialTarget.AdjacentCards.Any(card => adjacencySubtypes.All(s => card.SubtypeText.Contains(s)));
                case InAOE:             return Subeffect.Source.CardInAOE(potentialTarget);
                case WithinCSpacesOfSource: return potentialTarget.WithinSpaces(cSpaces, Subeffect.Source);
                case ExactlyXSpaces:    return potentialTarget.DistanceTo(Subeffect.Source) == Subeffect.Effect.X;
                case InFrontOfSource:   return Subeffect.Source.CardInFront(potentialTarget);
                case IndexInListGTEC:   return potentialTarget.IndexInList >= constant;
                case IndexInListLTEC:   return potentialTarget.IndexInList <= constant;
                case IndexInListLTEX:   return potentialTarget.IndexInList <= x;

                //misc
                case CanBePlayed: return Subeffect.Game.ExistsEffectPlaySpace(Subeffect.Source.PlayRestriction, Subeffect.Effect);
                case EffectControllerCanPayCost: return Subeffect.Effect.Controller.Pips >= potentialTarget.Cost * costMultiplier / costDivisor;
                default:
                    Debug.LogError($"You forgot to implement a check for {restriction}");
                    return false;
            }
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the given value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <param name="x">The value of X for which to consider this effect's restriction</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        public virtual bool Evaluate(GameCard potentialTarget, int x)
        {
            if (potentialTarget == null) return false;

            return cardRestrictions.All(r => RestrictionValid(r, potentialTarget, x));
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the current effect value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        public virtual bool Evaluate(GameCard potentialTarget) => Evaluate(potentialTarget, Subeffect.Effect.X);
    }
}