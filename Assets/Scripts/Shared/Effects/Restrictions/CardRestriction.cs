using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class CardRestriction : RestrictionBase<GameCardBase>
    {
        public Subeffect Subeffect => InitializationContext.subeffect;

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
        public const string Unique = "Unique";

        //different
        public const string DifferentFromSource = "Different from Source";
        public const string DifferentFromTarget = "Different from Target";
        public const string DifferentFromOtherTargets = "Different from Other Targets";
        public const string DifferentFromAugmentedCard = "Different from Augmented Card";

        //card types
        public const string Character = "Character";
        public const string Spell = "Spell";
        public const string Augment = "Augment";

        public const string NotCharacter = "Not Character";
        public const string NotAugment = "Not Augment";

        public const string SpellSubtypes = "Spell Subtypes";

        //control
        public const string Friendly = "Friendly";
        public const string Enemy = "Enemy";

        public const string SameOwner = "Same Owner as Source";
        public const string TurnPlayerControls = "Turn Player Controls";

        public const string ControllerMatchesCardTarget = "Controller Matches Card Target's";
        public const string ControllerDoesntMatchCardTarget = "Controller Doesn't Match Card Target's";
        public const string ControllerMatchesPlayerTarget = "Controller Matches Player Target";
        public const string ControllerIsntPlayerTarget = "Controller Isn't Player Target";

        //summoned
        public const string Summoned = "Summoned"; //non-avatar character
        public const string Avatar = "Avatar";
        public const string NotAvatar = "Not Avatar";

        //subtypes
        public const string SubtypesInclude = "Subtypes Include";
        public const string SubtypesExclude = "Subtypes Exclude";
        public const string SubtypesIncludeAnyOf = "Subtypes Include Any Of";

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
        public const string Revealed = "Revealed";

        //stats
        public const string CardValueFitsNumberRestriction = "Card Value Fits Number Restriction";

        //misc statlike
        public const string Hurt = "Hurt";
        public const string Unhurt = "Unhurt";

        public const string Negated = "Negated";
        public const string Unnegated = "Unnegated";
        public const string Activated = "Activated";

        public const string HasMovement = "Has Movement";
        public const string OutOfMovement = "Out of Movement";

        //positioning

        public const string SourceInThisAOE = "Source in This' AOE"; //whether the source card is in the potential target's aoe
        public const string CardHasCardRestrictionInAOE = "Card has Card Restriction in its AOE";
        public const string CardDoesntHaveCardRestrictionInAOE = "Card doesn't have Card Restriction in its AOE";
        public const string WouldBeInAOEOfCardTargetIfCardTargetWereAtSpaceTarget 
            = "Would Be In AOE Of Card Target If Card Target Were At Space Target";
        public const string WouldOverlapCardTargetIfCardTargetWereAtSpaceTarget
            = "Would Overlap Card Target If Card Target Were At Space Target";

        public const string IndexInListGTC = "Index>C";
        public const string IndexInListLTC = "Index<C";
        public const string IndexInListLTX = "Index<X";

        //misc
        public const string CanBePlayed = "Can Be Played";
        public const string CanPlayToTargetSpace = "Can be Played to Target Space";

        public const string EffectControllerCanPayCost = "Effect Controller can Afford Cost";

        #endregion restrictions

        public string[] cardRestrictions = { };

        public string nameIs;
        public string[] subtypesInclude;
        public string[] subtypesExclude;
        public string[] subtypesIncludeAnyOf;
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
        public CardRestriction hasInAOERestriction;
        public CardRestriction augmentRestriction;

        public SpaceRestriction spaceRestriction;

        public string[] canPlayIgnoring;

        public string blurb = "";

        public GameCard Source => InitializationContext.source;
        public Player Controller => InitializationContext.Controller;
        public Effect Effect => InitializationContext.effect;
        public Game Game => InitializationContext.game;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            Debug.Log($"Initializing card restriction with context {initializationContext}");
            base.Initialize(initializationContext);

            spaceRestriction?.Initialize(initializationContext);

            augmentRestriction?.Initialize(initializationContext);
            secondaryRestriction?.Initialize(initializationContext);
            adjacentCardRestriction?.Initialize(initializationContext);
            connectednessRestriction?.Initialize(initializationContext);
            attackedCardRestriction?.Initialize(initializationContext);
            inAOEOfRestriction?.Initialize(initializationContext);
            hasInAOERestriction?.Initialize(initializationContext);

            cardValueNumberRestriction?.Initialize(initializationContext);

            cardValue?.Initialize(initializationContext);
        }

        public override string ToString() => $"Card Restriction of {Source?.CardName}." +
            $"\nRestrictions: {string.Join(", ", cardRestrictions)}" +
            $"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";

        private bool HasCardRestrictionInAOE(GameCardBase cardToTest, IResolutionContext context)
        {
            if (cardToTest == null) return false;
            if (cardToTest.Location != CardLocation.Board) return false;

            return Source.Game.Cards.Any(c => cardToTest.CardInAOE(c) && hasInAOERestriction.IsValid(c, context));
        }

        private bool WouldCardBeInAOEOfCardTargetIfCardTargetWereAtSpaceTarget(GameCardBase cardToTest, GameCardBase cardTarget, Space space)
        {
            if (cardToTest == null) return false;
            if (space == null) return false;

            return cardTarget.CardInAOE(cardToTest, space);
        }

        private bool WouldCardOverlapCardTargetIfCardTargetWereAtSpaceTarget(GameCardBase cardToTest, GameCardBase cardTarget, Space space)
        {
            if (cardToTest == null) return false;
            if (space == null) return false;

            return cardTarget.Overlaps(cardToTest, space);
        }

        /// <summary>
        /// Determines whether the given restriction is true for a given card, with a given value of x.
        /// </summary>
        /// <param name="restriction">The restriction to check, as defined in the constants above.</param>
        /// <param name="potentialTarget">The card to consider the restriction for.</param>
        /// <param name="x">The value of x for which to consider the restriction</param>
        /// <param name="context">The activation context relevant here - the context for the Effect or for this triggering event</param>
        /// <returns><see langword="true"/> if the card fits the restriction for the given value of x, <see langword="false"/> otherwise.</returns>
        private bool IsRestrictionValid(string restriction, GameCardBase potentialTarget, IResolutionContext context)
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
                Unique => potentialTarget?.Unique ?? false,

                //different
                DifferentFromSource => potentialTarget?.Card != Source,
                DifferentFromTarget => potentialTarget?.Card != Subeffect.CardTarget,
                DifferentFromOtherTargets => Subeffect.Effect.CardTargets.All(c => !c.Equals(potentialTarget)),
                DifferentFromAugmentedCard => potentialTarget?.Card != Source.AugmentedCard,

                //card types
                Character => potentialTarget?.CardType == 'C',
                Spell => potentialTarget?.CardType == 'S',
                Augment => potentialTarget?.CardType == 'A',

                NotCharacter => potentialTarget?.CardType != 'C',
                NotAugment => potentialTarget?.CardType != 'A',
                SpellSubtypes => potentialTarget?.SpellSubtypes.Intersect(spellSubtypes).Any() ?? false,

                //control
                Friendly => potentialTarget?.Controller == Controller,
                Enemy => potentialTarget?.Controller != Controller,
                SameOwner => potentialTarget?.Owner == Controller,
                TurnPlayerControls => potentialTarget?.Controller == Subeffect.Game.TurnPlayer,

                ControllerMatchesCardTarget => potentialTarget?.Controller == Subeffect.CardTarget.Controller,
                ControllerDoesntMatchCardTarget => potentialTarget?.Controller != Subeffect.CardTarget.Controller,
                ControllerMatchesPlayerTarget => potentialTarget?.Controller == Subeffect.PlayerTarget,
                ControllerIsntPlayerTarget => potentialTarget?.Controller != Subeffect.PlayerTarget,

                //summoned
                Summoned => potentialTarget?.Summoned ?? false,
                Avatar => potentialTarget?.IsAvatar ?? false,
                NotAvatar => !potentialTarget?.IsAvatar ?? false,

                //subtypes
                SubtypesInclude => subtypesInclude.All(s => potentialTarget?.HasSubtype(s) ?? false),
                SubtypesExclude => subtypesExclude.All(s => !potentialTarget?.HasSubtype(s) ?? false),
                SubtypesIncludeAnyOf => subtypesIncludeAnyOf.Any(s => potentialTarget?.HasSubtype(s) ?? false),

                //is
                IsSource => potentialTarget?.Card == Source,
                NotSource => potentialTarget?.Card != Source,
                IsCardTarget => potentialTarget?.Card == Subeffect.CardTarget,
                NotContextCard => potentialTarget?.Card != context?.TriggerContext?.mainCardInfoBefore?.Card,

                AugmentsTarget => potentialTarget?.AugmentedCard == Subeffect.CardTarget,
                AugmentedBySource => potentialTarget?.Augments.Contains(Source) ?? false,
                //If the potential target is null, then it's not augmented by Source
                NotAugmentedBySource => !(potentialTarget?.Augments.Contains(Source) ?? true),

                AugmentsCardRestriction => augmentRestriction.IsValid(potentialTarget?.AugmentedCard, context),

                WieldsAugmentFittingRestriction => potentialTarget?.Augments.Any(c => augmentRestriction.IsValid(c, context)) ?? false,
                WieldsNoAugmentFittingRestriction => !(potentialTarget?.Augments.Any(c => augmentRestriction.IsValid(c, context)) ?? false),

                //location
                Hand => potentialTarget?.Location == CardLocation.Hand,
                Deck => potentialTarget?.Location == CardLocation.Deck,
                Discard => potentialTarget?.Location == CardLocation.Discard,
                Board => potentialTarget?.Location == CardLocation.Board,
                Annihilated => potentialTarget?.Location == CardLocation.Annihilation,
                LocationInList => locations.Contains(potentialTarget?.Location ?? CardLocation.Nowhere),
                Hidden => !potentialTarget?.KnownToEnemy ?? false,
                Revealed => potentialTarget?.KnownToEnemy ?? false,

                //stats
                CardValueFitsNumberRestriction => cardValueNumberRestriction.IsValidNumber(cardValue.GetValueOf(potentialTarget)),
                Hurt => potentialTarget?.Hurt ?? false,
                Unhurt => !(potentialTarget?.Hurt ?? true),
                Activated => potentialTarget?.Activated ?? false,
                Negated => potentialTarget?.Negated ?? false,
                Unnegated => !(potentialTarget?.Negated ?? true),
                HasMovement => potentialTarget?.SpacesCanMove > 0,
                OutOfMovement => potentialTarget?.SpacesCanMove <= 0,

                //positioning
                SourceInThisAOE => potentialTarget?.CardInAOE(Source) ?? false,
                CardHasCardRestrictionInAOE => HasCardRestrictionInAOE(potentialTarget, context),
                CardDoesntHaveCardRestrictionInAOE => !HasCardRestrictionInAOE(potentialTarget, context),
                WouldBeInAOEOfCardTargetIfCardTargetWereAtSpaceTarget 
                    => WouldCardBeInAOEOfCardTargetIfCardTargetWereAtSpaceTarget(potentialTarget, Subeffect.CardTarget, Subeffect.SpaceTarget),
                WouldOverlapCardTargetIfCardTargetWereAtSpaceTarget
                    => WouldCardOverlapCardTargetIfCardTargetWereAtSpaceTarget(potentialTarget, Subeffect.CardTarget, Subeffect.SpaceTarget),
                IndexInListGTC => potentialTarget?.IndexInList > constant,
                IndexInListLTC => potentialTarget?.IndexInList < constant,

                //fights
                IsDefendingFromSource
                    => Source.Game.StackEntries.Any(s => s is Attack atk && atk.attacker == Source && atk.defender == potentialTarget?.Card)
                    || (Source.Game.CurrStackEntry is Attack atk2 && atk2.attacker == Source && atk2.defender == potentialTarget?.Card),

                //misc
                Augmented => potentialTarget?.Augments.Any() ?? false,

                CanBePlayed
                    => Game.ExistsEffectPlaySpace(potentialTarget?.PlayRestriction, Effect),
                CanPlayToTargetSpace
                    => potentialTarget?.PlayRestriction.IsValidEffectPlay(Subeffect.SpaceTarget, Effect, Subeffect.PlayerTarget, context, ignoring: canPlayIgnoring) ?? false,

                EffectControllerCanPayCost => Subeffect.Effect.Controller.Pips >= potentialTarget?.Cost * costMultiplier / costDivisor,

                _ => throw new ArgumentException($"Invalid card restriction {restriction}", "restriction"),
            };

        /* This exists to debug a card restriction,
         * but should not be usually used because it prints a ton*/
        private bool IsRestrictionValidDebug(string restriction, GameCardBase potentialTarget, IResolutionContext context)
        {
            bool answer = IsRestrictionValid(restriction, potentialTarget, context);
            //if (!answer) Debug.Log($"{potentialTarget} flouts {restriction} in effect of {Source} in context {InitializationContext}");
            return answer;
        }

        protected override bool IsValidLogic(GameCardBase item, IResolutionContext context)
        {
            return base.IsValidLogic(item, context)
                && cardRestrictions.All(r => IsRestrictionValidDebug(r, item, context));
        }
    }
}