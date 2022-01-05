using KompasCore.Cards;
using KompasServer.Effects;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
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

        //different
        public const string DifferentFromSource = "Different from Source";
        public const string DifferentFromTarget = "Different from Target";
        public const string DifferentFromOtherTargets = "Different from Other Targets";
        public const string DifferentFromAugmentedCard = "Different from Augmented Card";

        //card types
        public const string IsCharacter = "Is Character";
        public const string IsSpell = "Is Spell";
        public const string IsAugment = "Is Augment";
        public const string NotAugment = "Not Augment";
        public const string Fast = "Fast";
        public const string SpellSubtypes = "Spell Subtypes";

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
        public const string NotSource = "Not Source";
        public const string IsTarget = "Is Target";
        public const string NotContextCard = "Not Context Card";
        public const string AugmentsTarget = "Augments Current Target";
        public const string AugmentedBySource = "Source Augments";
        public const string AugmentsCardRestriction = "Augments Card Fitting Restriction";
        public const string NotAugmentedBySource = "Source Doesn't Augment";
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
        public const string CardValueFitsXRestriction = "Card Value Fits X Restriction";
        //misc statlike
        public const string CanBeHealed = "Can Be Healed";

        public const string Negated = "Negated";
        public const string Activated = "Activated";
        public const string HasMovement = "Has Movement";
        public const string OutOfMovement = "Out of Movement";

        //positioning
        public const string SpaceFitsRestriction = "Space Fits Restriction";

        public const string SourceInThisAOE = "Source in This' AOE";

        public const string IndexInListGTC = "Index>C";
        public const string IndexInListLTC = "Index<C";
        public const string IndexInListLTX = "Index<X";

        //misc
        public const string CanBePlayed = "Can Be Played";
        //public const string TargetCanBePlayed = "Target Can Be Played";
        public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost";
        public const string Augmented = "Augmented";
        public const string IsDefendingFromSource = "Is Defending From Source";
        public const string CanPlayTargetToThisCharactersSpace = "Can Play Target to This Character's Space";
        public const string SpaceRestrictionValidIfThisTargetChosen = "Space Restriction Valid With This Target Chosen";
        public const string AttackingCardFittingRestriction = "Attacking Card Fitting Restriction";
        public const string EffectIsOnTheStack = "Effect is on the Stack";
        public const string CanPlayToTargetSpace = "Can be Played to Target Space";
        #endregion restrictions

        //because JsonConvert will fill in all values with defaults if not present
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
        public XRestriction xRestriction;

        public CardRestriction secondaryRestriction;

        public CardRestriction adjacentCardRestriction;
        public CardRestriction connectednessRestriction;
        public CardRestriction attackedCardRestriction;
        public CardRestriction inAOEOfRestriction;
        public CardRestriction augmentRestriction;

        public SpaceRestriction spaceRestriction;

        public GameCard Source { get; private set; }
        public Player Controller => Source == null ? null : Source.Controller;
        public Effect Effect { get; private set; }

        public string blurb = "";

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(Subeffect subeff)
        {
            this.Subeffect = subeff;
            Initialize(subeff.Source, subeff.Effect);
        }

        public void Initialize(GameCard source, Effect eff)
        {
            Source = source;
            Effect = eff;

            secondaryRestriction?.Initialize(source, eff);
            xRestriction?.Initialize(source, Subeffect);

            adjacentCardRestriction?.Initialize(source, eff);
            connectednessRestriction?.Initialize(source, eff);
            attackedCardRestriction?.Initialize(source, eff);
            inAOEOfRestriction?.Initialize(source, eff);

            cardValue?.Initialize(source);

            if (Subeffect != null) spaceRestriction?.Initialize(Subeffect);
            else spaceRestriction?.Initialize(source, eff.Controller, eff);

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
        private bool RestrictionValid(string restriction, GameCardBase potentialTarget, int x, ActivationContext context)
        {
            //if (potentialTarget == null) return false;
            //Debug.Log($"potential target controller? {potentialTarget.Controller}, my controller {Controller}");

            return restriction switch
            {
                //targets
                AlreadyTarget       => Effect.Targets.Contains(potentialTarget),
                NotAlreadyTarget    => !Effect.Targets.Contains(potentialTarget),

                //names
                NameIs                  => potentialTarget?.CardName == nameIs,
                SameName                => Subeffect.Target.CardName == potentialTarget?.CardName,
                SameNameAsSource        => potentialTarget?.CardName == Source.CardName,
                DistinctNameFromTargets => Effect.Targets.All(card => card.CardName != potentialTarget?.CardName),
                DistinctNameFromSource  => Source.CardName != potentialTarget?.CardName,

                //different
                DifferentFromSource         => potentialTarget?.Card != Source,
                DifferentFromTarget         => potentialTarget?.Card != Subeffect.Target,
                DifferentFromOtherTargets   => Subeffect.Effect.Targets.All(c => !c.Equals(potentialTarget)),
                DifferentFromAugmentedCard  => potentialTarget?.Card != Source.AugmentedCard,

                //card types
                IsCharacter     => potentialTarget?.CardType == 'C',
                IsSpell         => potentialTarget?.CardType == 'S',
                IsAugment       => potentialTarget?.CardType == 'A',
                NotAugment      => potentialTarget?.CardType != 'A',
                Fast            => potentialTarget?.Fast ?? false,
                SpellSubtypes   => potentialTarget?.SpellSubtypes.Intersect(spellSubtypes).Any() ?? false,

                //control
                Friendly            => potentialTarget?.Controller == Controller,
                Enemy               => potentialTarget?.Controller != Controller,
                SameOwner           => potentialTarget?.Owner == Controller,
                TurnPlayerControls  => potentialTarget?.Controller == Subeffect.Game.TurnPlayer,

                ControllerMatchesTarget         => potentialTarget?.Controller == Subeffect.Target.Controller,
                ControllerMatchesPlayerTarget   => potentialTarget?.Controller == Subeffect.Player,
                ControllerIsntPlayerTarget      => potentialTarget?.Controller != Subeffect.Player,

                //summoned
                Summoned    => potentialTarget?.Summoned ?? false,
                Avatar      => potentialTarget?.IsAvatar ?? false,
                NotAvatar   => !potentialTarget?.IsAvatar ?? false,

                //subtypes
                SubtypesInclude => subtypesInclude.All(s => potentialTarget?.SubtypeText.Contains(s) ?? false),
                SubtypesExclude => subtypesExclude.All(s => !potentialTarget?.SubtypeText.Contains(s) ?? false),

                //is
                IsSource        => potentialTarget?.Card == Source,
                NotSource       => potentialTarget?.Card != Source,
                IsTarget        => potentialTarget?.Card == Subeffect.Target,
                NotContextCard  => potentialTarget?.Card != context?.mainCardInfoBefore?.Card,

                AugmentsTarget      => potentialTarget?.AugmentedCard == Subeffect.Target,
                AugmentedBySource   => potentialTarget?.Augments.Contains(Source) ?? false,

                AugmentsCardRestriction => augmentRestriction.Evaluate(potentialTarget?.AugmentedCard, x, context),
                NotAugmentedBySource    => !(potentialTarget?.Augments.Contains(Source) ?? true),

                WieldsAugmentFittingRestriction     => potentialTarget?.Augments.Any(c => augmentRestriction.Evaluate(c, context)) ?? false,
                WieldsNoAugmentFittingRestriction   => !(potentialTarget?.Augments.Any(c => augmentRestriction.Evaluate(c, context)) ?? false),

                //location
                Hand            => potentialTarget?.Location == CardLocation.Hand,
                Deck            => potentialTarget?.Location == CardLocation.Deck,
                Discard         => potentialTarget?.Location == CardLocation.Discard,
                Board           => potentialTarget?.Location == CardLocation.Field,
                Annihilated     => potentialTarget?.Location == CardLocation.Annihilation,
                LocationInList  => locations.Contains(potentialTarget?.Location ?? CardLocation.Nowhere),
                Hidden          => !potentialTarget?.KnownToEnemy ?? false,

                //stats
                CardValueFitsXRestriction => xRestriction.Evaluate(cardValue.GetValueOf(potentialTarget)),
                CanBeHealed     => potentialTarget?.Hurt ?? false,
                Activated       => potentialTarget?.Activated ?? false,
                Negated         => potentialTarget?.Negated ?? false,
                HasMovement     => potentialTarget?.SpacesCanMove > 0,
                OutOfMovement   => potentialTarget?.SpacesCanMove <= 0,

                //positioning
                SpaceFitsRestriction => spaceRestriction.Evaluate(potentialTarget?.Position ?? Space.Nowhere, context),
                SourceInThisAOE => potentialTarget?.CardInAOE(Source) ?? false,
                IndexInListGTC => potentialTarget?.IndexInList > constant,
                IndexInListLTC => potentialTarget?.IndexInList < constant,
                IndexInListLTX => potentialTarget?.IndexInList < x,

                //fights
                AttackingCardFittingRestriction
                    => Source.Game.StackEntries.Any(e => e is Attack atk
                        && atk.attacker == potentialTarget?.Card
                        && attackedCardRestriction.Evaluate(atk.defender, context)),
                IsDefendingFromSource
                    => Source.Game.StackEntries.Any(s => s is Attack atk && atk.attacker == Source && atk.defender == potentialTarget?.Card)
                    || (Source.Game.CurrStackEntry is Attack atk2 && atk2.attacker == Source && atk2.defender == potentialTarget?.Card),

                //misc
                Augmented => potentialTarget?.Augments.Any() ?? false,

                CanBePlayed 
                    => Subeffect.Game.ExistsEffectPlaySpace(potentialTarget?.PlayRestriction, Effect),
                CanPlayToTargetSpace 
                    => potentialTarget?.PlayRestriction.EvaluateEffectPlay(Subeffect.Space, Effect, Subeffect.Player, context) ?? false,
                CanPlayTargetToThisCharactersSpace
                    => Subeffect.Target.PlayRestriction.EvaluateEffectPlay(potentialTarget?.Position ?? default, Effect, Subeffect.Player, context),

                EffectControllerCanPayCost => Subeffect.Effect.Controller.Pips >= potentialTarget?.Cost * costMultiplier / costDivisor,
                EffectIsOnTheStack => Source.Game.StackEntries.Any(e => e is Effect eff && eff.Source == potentialTarget?.Card),

                SpaceRestrictionValidIfThisTargetChosen 
                    => Effect.Subeffects[spaceRestrictionIndex] is SpaceTargetSubeffect spaceTgtSubeff 
                    && spaceTgtSubeff.WillBePossibleIfCardTargeted(theoreticalTarget: potentialTarget?.Card),

                _ => throw new ArgumentException($"Invalid card restriction {restriction}", "restriction"),
            };
        }

        /* This exists to debug a card restriction,
         * but should not be usually used because it prints a ton */
        public bool RestrictionValidDebug(string restriction, GameCardBase potentialTarget, int x, ActivationContext context)
        {
            bool answer = RestrictionValid(restriction, potentialTarget, x, context);
            if (!answer) Debug.Log($"{potentialTarget?.CardName} flouts {restriction}");
            return answer;
        }

        /// <summary>
        /// Checks whether the card in question fits the relevant retrictions, for the given value of X
        /// </summary>
        /// <param name="potentialTarget">The card to see if it fits all restrictions</param>
        /// <param name="x">The value of X for which to consider this effect's restriction</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        private bool Evaluate(GameCardBase potentialTarget, int x, ActivationContext context)
        {
            if (!initialized) throw new ArgumentException("Card restriction not initialized!");

            //if (potentialTarget == null) return false;

            try
            {
                return cardRestrictions.All(r => RestrictionValidDebug(r, potentialTarget, x, context));
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
        /// <param name="context">The activation context relevant here - the context for the Effect or for this triggering event</param>
        /// <returns><see langword="true"/> if the card fits all restrictions, <see langword="false"/> if it doesn't fit at least one</returns>
        public bool Evaluate(GameCardBase potentialTarget, ActivationContext context) 
            => Evaluate(potentialTarget, Subeffect?.Count ?? 0, context);
    }
}