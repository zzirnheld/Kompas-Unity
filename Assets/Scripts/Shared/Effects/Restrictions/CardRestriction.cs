using KompasCore.Cards;
using KompasServer.Effects;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    [Serializable]
    public class CardRestriction
    {
        [NonSerialized]
        private Subeffect subeffect;
        public Subeffect Subeffect { get => subeffect; private set => subeffect = value; }

        #region restrictions
        //targets
        public const string AlreadyTarget = "Already Target";
        public const string NotAlreadyTarget = "Not Already Target";

        //names
        public const string NameIs = "Name Is";
        public const string SameName = "Same Name as Target";
        public const string SameNameAsSource = "Same Name as Source";
        public const string DistinctNameFromTargets = "Distinct Name from Other Targets";
        public const string DistinctNameFromSource = "Distinct Name from Source";

        //card types
        public const string IsCharacter = "Is Character";
        public const string IsSpell = "Is Spell";
        public const string IsAugment = "Is Augment";
        public const string NotAugment = "Not Augment";
        public const string Fast = "Fast";

        //control
        public const string Friendly = "Friendly";
        public const string Enemy = "Enemy";
        public const string SameOwner = "Same Owner as Source";
        public const string TurnPlayerControls = "Turn Player Controls";
        public const string AdjacentToEnemy = "Adjacent to Enemy";
        public const string ControllerMatchesTarget = "Controller Matches Target's";
        public const string ControllerMatchesPlayerTarget = "Controller Matches Player Target";
        public const string ControllerIsntPlayerTarget = "Controller Isn't Player Target";

        //summoned
        public const string Summoned = "Summoned"; //non-avatar character
        public const string Avatar = "Avatar";
        public const string NotAvatar = "Not Avatar";

        //subtypes
        public const string SubtypesInclude = "Subtypes Include";
        public const string SubtypesExclude = "Subtypes Exclude";

        //is
        public const string IsSource = "Is Source";
        public const string AugmentsTarget = "Augments Current Target";
        public const string AugmentedBySource = "Source Augments";
        public const string WieldsAugmentFittingRestriction = "Wields Augment Fitting Restriction";

        //distinct
        public const string DistinctFromSource = "Distinct from Source";
        public const string DistinctFromTarget = "Distinct from Target";
        public const string DistinctFromAugmentedCard = "Distinct From Augmented Card";

        //location
        public const string Hand = "Hand";
        public const string Discard = "Discard";
        public const string Deck = "Deck";
        public const string Board = "Board";
        public const string Annihilated = "Annihilated";
        public const string LocationInList = "Multiple Possible Locations";

        public const string Hidden = "Hidden";

        //stats
        public const string CardValueFitsXRestriction = "Card Value Fits X Restriction";
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
        public const string CostLTX = "Cost<X";
            //<=C
        public const string NLTEC = "N<=C";
        public const string ELTEC = "E<=C";
        public const string SLTEC = "S<=C";
        public const string WLTEC = "W<=C";
        //>X
        public const string CostGTX = "Cost>X";
        //misc statlike
        public const string CostLTAvatar = "Cost<Avatar";
        public const string CostGTAvatar = "Cost>Avatar";
        public const string CanBeHealed = "Can Be Healed";

        public const string Negated = "Negated";

        //positioning
        public const string AdjacentToSource = "Adjacent to Source";
        public const string AdjacentToTarget = "Adjacent to Target";
        public const string AdjacentToCoords = "Adjacent to Coords";
        public const string WithinCSpacesOfSource = "Within C Spaces";
        public const string WithinCSpacesOfTarget = "Within C Spaces of Target";
        public const string WithinXSpacesOfSource = "Within X Spaces";
        public const string InAOE = "In AOE";
        public const string InTargetsAOE = "In Target's AOE";
        public const string NotInAOE = "Not In AOE";
        public const string AdjacentToSubtype = "Adjacent to Subtype";
        public const string ExactlyXSpaces = "Exactly X Spaces to Source";
        public const string InFrontOfSource = "In Front of Source";
        public const string BehindSource = "Behind Source";
        public const string IndexInListGTC = "Index>C";
        public const string IndexInListLTC = "Index<C";
        public const string IndexInListLTX = "Index<X";
        public const string SameColumnAsSource = "Same Column as Source";
        public const string DirectlyInFrontOfSource = "Directly In Front of Source";
        public const string InACorner = "In a Corner";
        public const string ConnectedToSourceBy = "Connected to Source By";
        public const string ConnectedToTargetBy = "Connected to Target By";

        //misc
        public const string CanBePlayed = "Can Be Played";
        public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost";
        public const string Augmented = "Augmented";
        public const string IsDefendingFromSource = "Is Defending From Source";
        public const string CanPlayTargetToThisCharactersSpace = "Can Play Target to This Character's Space";
        public const string SpaceRestrictionValidIfThisTargetChosen = "Space Restriction Valid With This Target Chosen";
        #endregion restrictions

        //because JsonUtility will fill in all values with defaults if not present
        public string[] cardRestrictions = new string[0];

        public string nameIs;
        public string[] subtypesInclude = new string[0];
        public string[] subtypesExclude = new string[0];
        public int constant;
        public CardLocation[] locations;
        public int costMultiplier = 1;
        public int costDivisor = 1;
        public int cSpaces;
        public string[] adjacencySubtypes = new string[0];
        public int spaceRestrictionIndex;

        public CardValue cardValue;
        public XRestriction xRestriction;

        public CardRestriction secondaryRestriction;

        public CardRestriction connectednessRestriction;

        public GameCard Source { get; private set; }
        public Player Controller { get; private set; }
        public Effect Effect { get; private set; }

        public string blurb = "";

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(Subeffect subeff)
        {
            this.Subeffect = subeff;
            Initialize(subeff.Source, subeff.Controller, subeff.Effect);
        }

        public void Initialize(GameCard source, Player controller, Effect eff)
        {
            Source = source;
            Controller = controller;
            Effect = eff;

            secondaryRestriction?.Initialize(source, controller, eff);
            xRestriction?.Initialize(source, Subeffect);

            /*
            if (cardRestrictions.Contains(ConnectedToSourceBy))
            {
                if (connectednessRestriction == null) Debug.LogError($"Couldn't load connectedness restriction");
                else Debug.Log($"Connectedness restriction: {connectednessRestriction}");
            }*/
            connectednessRestriction?.Initialize(source, controller, eff);

            initialized = true;
        }

        public override string ToString()
        {
            return $"Card Restriction.\nRestrictions: {string.Join(", ", cardRestrictions)}";
        }

        /// <summary>
        /// Determines whether the given restriction is true for a given card, with a given value of x.
        /// </summary>
        /// <param name="restriction">The restriction to check, as defined in the constants above.</param>
        /// <param name="potentialTarget">The card to consider the restriction for.</param>
        /// <param name="x">The value of x for which to consider the restriction</param>
        /// <returns><see langword="true"/> if the card fits the restriction for the given value of x, <see langword="false"/> otherwise.</returns>
        private bool RestrictionValid(string restriction, IGameCardInfo potentialTarget, int x)
        {
            if (potentialTarget == null) return false;

            switch (restriction)
            {
                //targets
                case AlreadyTarget:    return Effect.Targets.Contains(potentialTarget);
                case NotAlreadyTarget: return !Effect.Targets.Contains(potentialTarget);

                //names
                case NameIs:                  return potentialTarget.CardName == nameIs;
                case SameName:                return Subeffect.Target.CardName == potentialTarget.CardName;
                case SameNameAsSource:        return potentialTarget.CardName == Source.CardName;
                case DistinctNameFromTargets: return Effect.Targets.All(card => card.CardName != potentialTarget.CardName);
                case DistinctNameFromSource:  return Source.CardName != potentialTarget.CardName;

                //card types
                case IsCharacter: return potentialTarget.CardType == 'C';
                case IsSpell:     return potentialTarget.CardType == 'S';
                case IsAugment:   return potentialTarget.CardType == 'A';
                case NotAugment:  return potentialTarget.CardType != 'A';
                case Fast:        return potentialTarget.Fast;

                //control
                //Debug.Log($"potential target controller? {potentialTarget.Controller?.index}, my controller {Controller?.index}");
                case Friendly:  return potentialTarget.Controller == Controller;
                case Enemy:     return potentialTarget.Controller != Controller;
                case SameOwner: return potentialTarget.Owner == Controller;
                case TurnPlayerControls: return potentialTarget.Controller == Subeffect.Game.TurnPlayer;
                case ControllerMatchesTarget: return potentialTarget.Controller == Subeffect.Target.Controller;
                case ControllerMatchesPlayerTarget: return potentialTarget.Controller == Subeffect.Player;
                case ControllerIsntPlayerTarget: return potentialTarget.Controller != Subeffect.Player;

                //summoned
                case Summoned:  return potentialTarget.Summoned;
                case Avatar:    return potentialTarget.IsAvatar;
                case NotAvatar: return !potentialTarget.IsAvatar;

                //subtypes
                case SubtypesInclude: return subtypesInclude.All(s => potentialTarget.SubtypeText.Contains(s));
                case SubtypesExclude: return subtypesExclude.All(s => !potentialTarget.SubtypeText.Contains(s));

                //is
                case IsSource: return potentialTarget.Card == Source;
                case AugmentsTarget: return potentialTarget.AugmentedCard == Subeffect.Target;
                case AugmentedBySource: return potentialTarget.Augments.Contains(Source);
                case WieldsAugmentFittingRestriction: return potentialTarget.Augments.Any(c => secondaryRestriction.Evaluate(c));

                //distinct
                case DistinctFromSource: return potentialTarget.Card != Source;
                case DistinctFromTarget: return potentialTarget.Card != Subeffect.Target;
                case DistinctFromAugmentedCard: return potentialTarget.Card != Source.AugmentedCard;

                //location
                case Hand:           return potentialTarget.Location == CardLocation.Hand;
                case Deck:           return potentialTarget.Location == CardLocation.Deck;
                case Discard:        return potentialTarget.Location == CardLocation.Discard;
                case Board:          return potentialTarget.Location == CardLocation.Field;
                case Annihilated:    return potentialTarget.Location == CardLocation.Annihilation;
                case LocationInList: return locations.Contains(potentialTarget.Location);

                case Hidden:         return !potentialTarget.KnownToEnemy;

                //stats
                case CardValueFitsXRestriction: return xRestriction.Evaluate(cardValue.GetValueOf(potentialTarget));
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
                case CostLTX:  return potentialTarget.Cost < x;
                    //<=C
                case NLTEC:    return potentialTarget.N <= constant;
                case ELTEC:    return potentialTarget.E <= constant;
                case SLTEC:    return potentialTarget.S <= constant;
                case WLTEC:    return potentialTarget.W <= constant;
                case CostGTX:  return potentialTarget.Cost > x;
                    //misc
                case CostLTAvatar: return potentialTarget.Cost < Source.Controller.Avatar.Cost;
                case CostGTAvatar: return potentialTarget.Cost > Source.Controller.Avatar.Cost;
                case CanBeHealed: return potentialTarget.CardType == 'C' && potentialTarget.Location == CardLocation.Field 
                        && potentialTarget.E < potentialTarget.BaseE;

                case Negated:  return potentialTarget.Negated;

                //positioning
                case AdjacentToSource:           return potentialTarget.IsAdjacentTo(Source);
                case AdjacentToTarget:   return potentialTarget.IsAdjacentTo(Subeffect.Target);
                case AdjacentToCoords:   return potentialTarget.IsAdjacentTo(Subeffect.Space);
                case AdjacentToSubtype:  return potentialTarget.AdjacentCards.Any(card => adjacencySubtypes.All(s => card.SubtypeText.Contains(s)));
                case InAOE:              return Source.CardInAOE(potentialTarget);
                case InTargetsAOE:       return Subeffect.Target.CardInAOE(potentialTarget);
                case NotInAOE:           return !Source.CardInAOE(potentialTarget);
                case WithinCSpacesOfSource: return potentialTarget.WithinSpaces(cSpaces, Source);
                case WithinCSpacesOfTarget: return potentialTarget.WithinSpaces(cSpaces, Subeffect.Target);
                case WithinXSpacesOfSource: return potentialTarget.WithinSpaces(Effect.X, Source);
                case ExactlyXSpaces:     return potentialTarget.DistanceTo(Source) == x;
                case InFrontOfSource:    return Source.CardInFront(potentialTarget);
                case BehindSource:       return Source.CardBehind(potentialTarget);
                case IndexInListGTC:     return potentialTarget.IndexInList > constant;
                case IndexInListLTC:     return potentialTarget.IndexInList < constant;
                case IndexInListLTX:     return potentialTarget.IndexInList < x;
                case SameColumnAsSource: return potentialTarget.SameColumn(Source);
                case DirectlyInFrontOfSource: return Source.CardDirectlyInFront(potentialTarget);
                case InACorner:          return potentialTarget.InCorner();
                case ConnectedToSourceBy:
                    return potentialTarget.ShortestPath(Source.BoardX, Source.BoardY, connectednessRestriction.Evaluate) < 50; 
                case ConnectedToTargetBy: 
                    return potentialTarget.ShortestPath(Subeffect.Target.BoardX, Subeffect.Target.BoardY, connectednessRestriction.Evaluate) < 50; 

                //misc
                case CanBePlayed: return Subeffect.Game.ExistsEffectPlaySpace(Source.PlayRestriction, Effect);
                case EffectControllerCanPayCost: return Subeffect.Effect.Controller.Pips >= potentialTarget.Cost * costMultiplier / costDivisor;
                case Augmented: return potentialTarget.Augments.Any();
                case IsDefendingFromSource:
                    return Source.Game.StackEntries.Any(s => s is Attack atk && atk.attacker == Source && atk.defender == potentialTarget.Card)
                        || (Source.Game.CurrStackEntry is Attack atk2 && atk2.attacker == Source && atk2.defender == potentialTarget.Card);
                case CanPlayTargetToThisCharactersSpace:
                    return Subeffect.Target.PlayRestriction.EvaluateEffectPlay(potentialTarget.Position.x, potentialTarget.Position.y, Effect);
                case SpaceRestrictionValidIfThisTargetChosen:
                    if (Effect.Subeffects[spaceRestrictionIndex] is SpaceTargetSubeffect spaceTgtSubeff)
                        return spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: potentialTarget.Card);
                    else return false;
                default: throw new ArgumentException($"Invalid card restriction {restriction}", "restriction");
            }
        }

        /* This exists to debug a card restriction,
         * but should not be usually used because it prints a ton */
        public bool RestrictionValidDebug(string restriction, IGameCardInfo potentialTarget, int x)
        {
            bool answer = RestrictionValid(restriction, potentialTarget, x);
            if (!answer) Debug.Log($"{potentialTarget.CardName} flouts {restriction}");
            return answer;
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the given value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <param name="x">The value of X for which to consider this effect's restriction</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        private bool Evaluate(IGameCardInfo potentialTarget, int x)
        {
            if (!initialized) throw new ArgumentException("Card restriction not initialized!");

            if (potentialTarget == null) return false;

            try
            {
                return cardRestrictions.All(r => RestrictionValid(r, potentialTarget, x));
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e);
                return false;
            }
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the current effect value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        public bool Evaluate(IGameCardInfo potentialTarget) => Evaluate(potentialTarget, Subeffect?.Count ?? 0);
    }
}