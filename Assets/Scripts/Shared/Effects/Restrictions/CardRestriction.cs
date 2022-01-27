using KompasCore.Cards;
using KompasServer.Effects;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class CardRestriction
    {
        public Subeffect Subeffect { get; private set; }

        #region restrictions
        //targets
        public const string AlreadyTarget = "Already Target";
        public const string NotAlreadyTarget = "Not Already Target";

        //names
        public const string NameIs = "Name Is";
        public const string SameNameAsTarget = "Same Name as Target";
        public const string SameNameAsSource = "Same Name as Source";
        public const string DistinctNameFromTargets = "Distinct Name from Other Targets";
        public const string DistinctNameFromSource = "Distinct Name from Source";

        //different
        public const string DifferentFromSource = "Different from Source";
        public const string DifferentFromTarget = "Different from Target";
        public const string DifferentFromOtherTargets = "Different from Other Targets";
        public const string DifferentFromAugmentedCard = "Different from Augmented Card";

        //card types
        public const string Character = "Character";
        public const string Spell = "Spell";
        public const string Augment = "Augment";
        public const string NotAugment = "Not Augment";
        public const string SpellSubtypes = "Spell Subtypes";

        //control
        public const string Friendly = "Friendly";
        public const string Enemy = "Enemy";

        public const string SameOwner = "Same Owner as Source";
        public const string TurnPlayerControls = "Turn Player Controls";

        public const string ControllerMatchesCardTarget = "Controller Matches Card Target's";
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
        public const string NotSource = "Not Source";
        public const string IsCardTarget = "Is Card Target";
        public const string NotContextCard = "Not Context Card";

        public const string Augmented = "Augmented";
        public const string AugmentsTarget = "Augments Current Target";
        public const string AugmentedBySource = "Source Augments";
        public const string NotAugmentedBySource = "Source Doesn't Augment";
        public const string AugmentsCardRestriction = "Augments Card Fitting Restriction";

        public const string WieldsAugmentFittingRestriction = "Wields Augment Fitting Restriction";
        public const string WieldsNoAugmentFittingRestriction = "Wields No Augment Fitting Restriction";

        //location
        public const string Hand = "Hand";
        public const string Discard = "Discard";
        public const string Deck = "Deck";
        public const string Board = "Board";
        public const string Annihilated = "Annihilated";
        public const string LocationInList = "Multiple Possible Locations";

        public const string Hidden = "Hidden";

        //stats
        public const string CardValueFitsNumberRestriction = "Card Value Fits Number Restriction";

        //misc statlike
        public const string Hurt = "Hurt";

        public const string Negated = "Negated";
        public const string Activated = "Activated";

        public const string HasMovement = "Has Movement";
        public const string OutOfMovement = "Out of Movement";

        //positioning
        public const string SpaceFitsRestriction = "Space Fits Restriction";

        public const string SourceInThisAOE = "Source in This' AOE"; //whether the source card is in the potential target's aoe

        public const string IndexInListGTC = "Index>C";
        public const string IndexInListLTC = "Index<C";
        public const string IndexInListLTX = "Index<X";

        //misc
        public const string CanBePlayed = "Can Be Played";
        public const string CanPlayToTargetSpace = "Can be Played to Target Space";

        public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost";
        public const string IsDefendingFromSource = "Is Defending From Source";
        public const string CanPlayTargetToThisCharactersSpace = "Can Play Target to This Character's Space";
        public const string SpaceRestrictionValidIfThisTargetChosen = "Space Restriction Valid With This Target Chosen";
        public const string AttackingCardFittingRestriction = "Attacking Card Fitting Restriction";
        public const string EffectIsOnTheStack = "Effect is on the Stack";
        #endregion restrictions

        public string[] cardRestrictions = new string[0];

        public string nameIs;
        public string[] subtypesInclude;
        public string[] subtypesExclude;
        public int constant;
        public CardLocation[] locations;
        public string[] spellSubtypes;

        //used for "can afford cost"
        public int costMultiplier = 1;
        public int costDivisor = 1;

        //used for "space restriction valid if this target chosen"
        public int spaceRestrictionIndex;

        public CardValue cardValue;
        public NumberRestriction cardValueNumberRestriction;

        public CardRestriction secondaryRestriction;

        public CardRestriction adjacentCardRestriction;
        public CardRestriction connectednessRestriction;
        public CardRestriction attackedCardRestriction;
        public CardRestriction inAOEOfRestriction;
        public CardRestriction augmentRestriction;

        public SpaceRestriction spaceRestriction;

        public GameCard Source { get; private set; }
        public Player Controller => Effect?.Controller ?? Source?.Controller;
        public Effect Effect { get; private set; }

        public string blurb = "";

        private bool initialized = false;

        /// <summary>
        /// Initializes this card restriction to match information found on this subeffect.
        /// </summary>
        /// <param name="subeff"></param>
        public void Initialize(Subeffect subeff) => Initialize(subeff.Source, subeff.Effect, subeff);

        /// <summary>
        /// Initializes this card restriction to match the given information.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="effect"></param>
        public void Initialize(GameCard source, Effect effect, Subeffect subeffect)
        {
            Subeffect = subeffect;
            Source = source;
            Effect = effect;

            spaceRestriction?.Initialize(source, effect.Controller, effect, subeffect);

            secondaryRestriction?.Initialize(source, effect, subeffect);
            adjacentCardRestriction?.Initialize(source, effect, subeffect);
            connectednessRestriction?.Initialize(source, effect, subeffect);
            attackedCardRestriction?.Initialize(source, effect, subeffect);
            inAOEOfRestriction?.Initialize(source, effect, subeffect);

            cardValueNumberRestriction?.Initialize(source, subeffect);

            cardValue?.Initialize(source);

            initialized = true;
            Debug.Log($"Initialized {this}");
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
        /// <param name="context">The activation context relevant here - the context for the Effect or for this triggering event</param>
        /// <returns><see langword="true"/> if the card fits the restriction for the given value of x, <see langword="false"/> otherwise.</returns>
        private bool IsRestrictionValid(string restriction, GameCardBase potentialTarget, int x, ActivationContext context) 
            => restriction != null && restriction switch
        {
            //targets
            AlreadyTarget => Effect.CardTargets.Contains(potentialTarget),
            NotAlreadyTarget => !Effect.CardTargets.Contains(potentialTarget),

            //names
            NameIs => potentialTarget?.CardName == nameIs,
            SameNameAsTarget => Subeffect.CardTarget.CardName == potentialTarget?.CardName,
            SameNameAsSource => potentialTarget?.CardName == Source.CardName,
            DistinctNameFromTargets => Effect.CardTargets.All(card => card.CardName != potentialTarget?.CardName),
            DistinctNameFromSource => Source.CardName != potentialTarget?.CardName,

            //different
            DifferentFromSource => potentialTarget?.Card != Source,
            DifferentFromTarget => potentialTarget?.Card != Subeffect.CardTarget,
            DifferentFromOtherTargets => Subeffect.Effect.CardTargets.All(c => !c.Equals(potentialTarget)),
            DifferentFromAugmentedCard => potentialTarget?.Card != Source.AugmentedCard,

            //card types
            Character   => potentialTarget?.CardType == 'C',
            Spell       => potentialTarget?.CardType == 'S',
            Augment     => potentialTarget?.CardType == 'A',

            NotAugment => potentialTarget?.CardType != 'A',
            SpellSubtypes => potentialTarget?.SpellSubtypes.Intersect(spellSubtypes).Any() ?? false,

            //control
            Friendly    => potentialTarget?.Controller == Controller,
            Enemy       => potentialTarget?.Controller != Controller,
            SameOwner   => potentialTarget?.Owner == Controller,
            TurnPlayerControls  => potentialTarget?.Controller == Subeffect.Game.TurnPlayer,

            ControllerMatchesCardTarget         => potentialTarget?.Controller == Subeffect.CardTarget.Controller,
            ControllerMatchesPlayerTarget   => potentialTarget?.Controller == Subeffect.PlayerTarget,
            ControllerIsntPlayerTarget      => potentialTarget?.Controller != Subeffect.PlayerTarget,

            //summoned
            Summoned    => potentialTarget?.Summoned ?? false,
            Avatar      => potentialTarget?.IsAvatar ?? false,
            NotAvatar   => !potentialTarget?.IsAvatar ?? false,

            //subtypes
            SubtypesInclude => subtypesInclude.All(s => potentialTarget?.SubtypeText.Contains(s) ?? false),
            SubtypesExclude => subtypesExclude.All(s => !potentialTarget?.SubtypeText.Contains(s) ?? false),

            //is
            IsSource => potentialTarget?.Card == Source,
            NotSource => potentialTarget?.Card != Source,
            IsCardTarget => potentialTarget?.Card == Subeffect.CardTarget,
            NotContextCard => potentialTarget?.Card != context?.mainCardInfoBefore?.Card,

            AugmentsTarget => potentialTarget?.AugmentedCard == Subeffect.CardTarget,
            AugmentedBySource => potentialTarget?.Augments.Contains(Source) ?? false,
            //If the potential target is null, then it's not augmented by Source
            NotAugmentedBySource => !(potentialTarget?.Augments.Contains(Source) ?? true),

            AugmentsCardRestriction => augmentRestriction.IsValidCard(potentialTarget?.AugmentedCard, x, context),

            WieldsAugmentFittingRestriction => potentialTarget?.Augments.Any(c => augmentRestriction.IsValidCard(c, context)) ?? false,
            WieldsNoAugmentFittingRestriction => !(potentialTarget?.Augments.Any(c => augmentRestriction.IsValidCard(c, context)) ?? false),

            //location
            Hand => potentialTarget?.Location == CardLocation.Hand,
            Deck => potentialTarget?.Location == CardLocation.Deck,
            Discard => potentialTarget?.Location == CardLocation.Discard,
            Board => potentialTarget?.Location == CardLocation.Board,
            Annihilated => potentialTarget?.Location == CardLocation.Annihilation,
            LocationInList => locations.Contains(potentialTarget?.Location ?? CardLocation.Nowhere),
            Hidden => !potentialTarget?.KnownToEnemy ?? false,

            //stats
            CardValueFitsNumberRestriction => cardValueNumberRestriction.IsValidNumber(cardValue.GetValueOf(potentialTarget)),
            Hurt => potentialTarget?.Hurt ?? false,
            Activated => potentialTarget?.Activated ?? false,
            Negated => potentialTarget?.Negated ?? false,
            HasMovement => potentialTarget?.SpacesCanMove > 0,
            OutOfMovement => potentialTarget?.SpacesCanMove <= 0,

            //positioning
            SpaceFitsRestriction => spaceRestriction.IsValidSpace(potentialTarget?.Position ?? Space.Nowhere, context),
            SourceInThisAOE => potentialTarget?.CardInAOE(Source) ?? false,
            IndexInListGTC => potentialTarget?.IndexInList > constant,
            IndexInListLTC => potentialTarget?.IndexInList < constant,
            IndexInListLTX => potentialTarget?.IndexInList < x,

            //fights
            AttackingCardFittingRestriction
                => Source.Game.StackEntries.Any(e => e is Attack atk
                    && atk.attacker == potentialTarget?.Card
                    && attackedCardRestriction.IsValidCard(atk.defender, context)),
            IsDefendingFromSource
                => Source.Game.StackEntries.Any(s => s is Attack atk && atk.attacker == Source && atk.defender == potentialTarget?.Card)
                || (Source.Game.CurrStackEntry is Attack atk2 && atk2.attacker == Source && atk2.defender == potentialTarget?.Card),

            //misc
            Augmented => potentialTarget?.Augments.Any() ?? false,

            CanBePlayed
                => Subeffect.Game.ExistsEffectPlaySpace(potentialTarget?.PlayRestriction, Effect),
            CanPlayToTargetSpace
                => potentialTarget?.PlayRestriction.IsValidEffectPlay(Subeffect.SpaceTarget, Effect, Subeffect.PlayerTarget, context) ?? false,
            CanPlayTargetToThisCharactersSpace
                => Subeffect.CardTarget.PlayRestriction.IsValidEffectPlay(potentialTarget?.Position ?? default, Effect, Subeffect.PlayerTarget, context),

            EffectControllerCanPayCost => Subeffect.Effect.Controller.Pips >= potentialTarget?.Cost * costMultiplier / costDivisor,
            EffectIsOnTheStack => Source.Game.StackEntries.Any(e => e is Effect eff && eff.Source == potentialTarget?.Card),

            SpaceRestrictionValidIfThisTargetChosen
                => Effect.Subeffects[spaceRestrictionIndex] is SpaceTargetSubeffect spaceTgtSubeff
                && spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: potentialTarget?.Card),

            _ => throw new ArgumentException($"Invalid card restriction {restriction}", "restriction"),
        };

        /* This exists to debug a card restriction,
         * but should not be usually used because it prints a ton*/
        public bool IsRestrictionValidDebug(string restriction, GameCardBase potentialTarget, int x, ActivationContext context)
        {
            bool answer = IsRestrictionValid(restriction, potentialTarget, x, context);
            if (!answer) Debug.Log($"{potentialTarget?.CardName} flouts {restriction}");
            return answer;
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the given value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <param name="x">The value of X for which to consider this effect's restriction</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        private bool IsValidCard(GameCardBase potentialTarget, int x, ActivationContext context)
        {
            if (!initialized) throw new ArgumentException("Card restriction not initialized!");

            try
            {
                return cardRestrictions.All(r => IsRestrictionValid(r, potentialTarget, x, context));
            }
            catch (ArgumentException ae)
            {
                Debug.LogError(ae);
                return false;
            }
            catch (NullReferenceException nre)
            {
                Debug.LogError(nre);
                return false;
            }
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the current effect value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <param name="context">The activation context relevant here - the context for the Effect or for this triggering event</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        public bool IsValidCard(GameCardBase potentialTarget, ActivationContext context) 
            => IsValidCard(potentialTarget, Subeffect?.Count ?? 0, context);
    }
}