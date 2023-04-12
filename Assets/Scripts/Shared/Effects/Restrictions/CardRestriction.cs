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
        #endregion restrictions

        public string[] cardRestrictions = { };

        public string nameIs;
        public string[] subtypesInclude;
        public string[] subtypesExclude;
        public string[] subtypesIncludeAnyOf;
        public CardLocation[] locations;
        public string[] spellSubtypes;

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

        public string blurb = "";

        public GameCard Source => InitializationContext.source;
        public Player Controller => InitializationContext.Controller;
        public Effect Effect => InitializationContext.effect;

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

                //misc
                Augmented => potentialTarget?.Augments.Any() ?? false,

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