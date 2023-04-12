using KompasCore.Cards;
using System;
using System.Linq;

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
        #endregion restrictions

        public string[] cardRestrictions = { };

        public string nameIs;
        public string[] subtypesInclude;
        public string[] subtypesExclude;
        public string[] subtypesIncludeAnyOf;
        public string[] spellSubtypes;

        public string blurb = "";

        public GameCard Source => InitializationContext.source;
        public Player Controller => InitializationContext.Controller;
        public Effect Effect => InitializationContext.effect;

        public override string ToString() => $"Card Restriction of {Source?.CardName}." +
            $"\nRestrictions: {string.Join(", ", cardRestrictions)}" +
            $"\nRestriction Elements: {string.Join(", ", elements.Select(r => r))}";

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