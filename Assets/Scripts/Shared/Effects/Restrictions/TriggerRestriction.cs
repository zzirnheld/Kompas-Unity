using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class TriggerRestriction
    {
        public Game Game { get; private set; }

        #region trigger restrictions
        private const string ThisCardIsMainCard = "This Card is Main Card";
        private const string ThisCardIsSecondaryCard = "This Card is Secondary Card";
        private const string AugmentedCardIsMainCard = "Augmented Card is Main Card";

        //This is technically redundant, but so gosh darn common that I'm not deprecating it
        private const string ThisCardInPlay = "This Card in Play";

        //Now: At the moment the trigger is evaluated, aka after the triggering event('s stackable, if any,) resolves
        //Before: Immediately before the triggering event
        //After: Immediately after the triggering event
        private const string ThisCardFitsRestriction = "This Card Fits Restriction Now";

        private const string StackableSourceFitsRestriction = "Stackable Source Fits Restriction Now";
        private const string MainCardIsStackableSource = "Main Card is Stackable Source";

        private const string MainCardFitsRestrictionBefore = "Main Card Fits Restriction Before";
        private const string MainCardsAugmentedCardBeforeFitsRestriction = "Main Card's Augmented Card Before Fits Restriction";
        private const string MainCardFitsRestrictionAfter = "Main Card Fits Restriction After";
        private const string MainCardIsASecondaryContextCardTarget = "Main Card is a Secondary Context Card Target";
        private const string MainCardAfterFurtherFromSourceThanBefore = "Main Card After is Further from Source than Before";

        private const string SecondaryCardFitsRestrictionBefore = "Secondary Card Fits Restriction Before";

        private const string CardExistsNow = "Card Exists Now";

        private const string SpaceFitsRestriction = "Space Fits Restriction";

        private const string XFitsRestriction = "X Fits Restriction";
        private const string StackableSourceIsMainCard = "Stackable Source is Main Card";
        private const string StackableSourceNotThisEffect = "Stackable Source isn't This Effect";

        private const string MainCardIsSecondaryDelayedCardTarget = "Main Card is Secondary Delayed Card Target";
        private const string SpaceIsSecondaryDelayedSpaceTarget = "Space is Secondary Delayed Space Target";
        private const string StackableIsSecondaryDelayedStackableTarget = "Stackable is Secondary Delayed Stackable Target";

        private const string NoStackable = "No Stackable"; //Aka "Normally"
        private const string NotFromEffect = "Not From Effect"; //But can be from attack
        private const string FromAttack = "From Attack";

        private const string ContextsStackablesMatch = "Contexts Stackables Match";
        private const string StackableIsThisEffect = "Stackable is This Effect";
        private const string StackableIsASecondaryContextStackableTarget = "Stackable is Secondary Context Stackable Target";

        private const string ControllerTriggered = "Controller Triggered";
        private const string EnemyTriggered = "Enemy Triggered";

        //Turns are checked "now", aka when the triggering event('s stackable, if any,) has resolved
        private const string FriendlyTurn = "Friendly Turn";
        private const string EnemyTurn = "Enemy Turn";

        private const string FromField = "From Field";
        private const string FromDeck = "From Deck";

        private const string MaxPerTurn = "Max Per Turn";
        private const string MaxPerRound = "Max Per Round";
        private const string MaxPerStack = "Max Per Stack";
        #endregion trigger conditions

        private static readonly string[] RequiringCardRestriction = 
            { MainCardFitsRestrictionBefore, SecondaryCardFitsRestrictionBefore, MainCardsAugmentedCardBeforeFitsRestriction };
        private static readonly string[] RequiringNowRestriction = { MainCardFitsRestrictionAfter };
        private static readonly string[] RequiringSelfRestriction = { ThisCardFitsRestriction };
        private static readonly string[] RequiringExistsRestriction = { CardExistsNow };
        private static readonly string[] RequiringSourceRestriction = { StackableSourceFitsRestriction };
        private static readonly string[] RequiringNumberRestriction = { XFitsRestriction };
        private static readonly string[] RequiringSpaceRestriction = { SpaceFitsRestriction };

        private static readonly ISet<string> ReevalationRestrictions = new HashSet<string>(new string[] { MaxPerTurn, MaxPerRound, MaxPerStack });

        public static readonly string[] DefaultFallOffRestrictions = { ThisCardIsMainCard, ThisCardInPlay };

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction;
        public CardRestriction nowRestriction;
        public CardRestriction secondaryCardRestriction;
        public CardRestriction adjacencyRestriction;
        public CardRestriction selfRestriction;
        public CardRestriction sourceRestriction;
        public CardRestriction existsRestriction;
        public NumberRestriction xRestriction;
        public SpaceRestriction spaceRestriction;

        public int maxTimesPerTurn = 1;
        public int maxPerRound = 1;
        public int maxPerStack = 1;
        public int distance = 1;

        public TriggerRestrictionElement[] triggerRestrictionElements = { };

        public GameCard ThisCard { get; private set; }

        public Trigger ThisTrigger { get; private set; }
        public Effect SourceEffect { get; private set; }
        public Subeffect Subeffect { get; private set; }

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(Game game, GameCard thisCard, Trigger thisTrigger, Effect effect, Subeffect subeffect = default)
        {
            Game = game;
            ThisCard = thisCard;
            ThisTrigger = thisTrigger;
            SourceEffect = effect;

            cardRestriction?.Initialize(thisCard, effect, subeffect);
            nowRestriction?.Initialize(thisCard, effect, subeffect);
            secondaryCardRestriction?.Initialize(thisCard, effect, subeffect);
            existsRestriction?.Initialize(thisCard, effect, subeffect);
            selfRestriction?.Initialize(thisCard, effect, subeffect);
            spaceRestriction?.Initialize(thisCard, thisCard.Controller, effect, subeffect);
            sourceRestriction?.Initialize(thisCard, effect, subeffect);
            xRestriction?.Initialize(thisCard);

            foreach (var tre in triggerRestrictionElements)
            {
                tre.Initialize(new RestrictionContext(game: Game, source: thisCard, subeffect: subeffect));
            }
            
            //Verify that any relevant restrictions exist
            if (triggerRestrictions.Intersect(RequiringCardRestriction).Any() && cardRestriction == null)
                throw new ArgumentNullException("cardRestriction", $"Must be populated for any of these restrictions: {RequiringCardRestriction}");
            if (triggerRestrictions.Intersect(RequiringNowRestriction).Any() && nowRestriction == null)
                throw new ArgumentNullException("nowRestriction", $"Must be populated for any of these restrictions: {RequiringNowRestriction}");
            if (triggerRestrictions.Intersect(RequiringSelfRestriction).Any() && selfRestriction == null)
                throw new ArgumentNullException("selfRestriction", $"Must be populated for any of these restrictions: {RequiringSelfRestriction}");
            if (triggerRestrictions.Intersect(RequiringExistsRestriction).Any() && existsRestriction == null)
                throw new ArgumentNullException("existsRestriction", $"Must be populated for any of these restrictions: {RequiringExistsRestriction}");
            if (triggerRestrictions.Intersect(RequiringSourceRestriction).Any() && sourceRestriction == null)
                throw new ArgumentNullException("sourceRestriction", $"Must be populated for any of these restrictions: {RequiringSourceRestriction}");
            if (triggerRestrictions.Intersect(RequiringNumberRestriction).Any() && xRestriction == null)
                throw new ArgumentNullException("xRestriction", $"Must be populated for any of these restrictions: {RequiringNumberRestriction}");
            if (triggerRestrictions.Intersect(RequiringSpaceRestriction).Any() && spaceRestriction == null)
                throw new ArgumentNullException("spaceRestriction", $"Must be populated for any of these restrictions: {RequiringSpaceRestriction}");

            initialized = true;
            //Debug.Log($"Initializing trigger restriction for {thisCard?.CardName}. game is null? {game}");
        }


        private bool IsRestrictionValid(string restriction, ActivationContext context, ActivationContext secondary = default) => restriction switch
        {
            //card triggering stuff
            ThisCardIsMainCard => context.mainCardInfoBefore?.Card == ThisCard,
            ThisCardIsSecondaryCard => context.secondaryCardInfoBefore.Card == ThisCard,
            AugmentedCardIsMainCard => context.mainCardInfoBefore.Augments.Contains(ThisCard),

            ThisCardInPlay => ThisCard.Location == CardLocation.Board,
            CardExistsNow => ThisCard.Game.Cards.Any(c => existsRestriction.IsValidCard(c, context)),

            ThisCardFitsRestriction => selfRestriction.IsValidCard(ThisCard, context),

            MainCardFitsRestrictionBefore => cardRestriction.IsValidCard(context.mainCardInfoBefore, context),
            SecondaryCardFitsRestrictionBefore => secondaryCardRestriction.IsValidCard(context.secondaryCardInfoBefore, context),
            MainCardFitsRestrictionAfter => nowRestriction.IsValidCard(context.MainCardInfoAfter, context),
            MainCardsAugmentedCardBeforeFitsRestriction => cardRestriction.IsValidCard(context.mainCardInfoBefore.AugmentedCard, context),
            MainCardIsASecondaryContextCardTarget => secondary?.CardTargets?.Any(c => c == context.mainCardInfoBefore?.Card) ?? false,

            MainCardIsStackableSource => context.stackableCause?.Source == context.mainCardInfoBefore.Card,
            StackableSourceFitsRestriction => sourceRestriction.IsValidCard(context.stackableCause?.Source, context),
            StackableSourceNotThisEffect => context.stackableCause != SourceEffect,
            ContextsStackablesMatch => context.stackableCause == secondary?.stackableCause,
            StackableIsThisEffect => context.stackableCause == SourceEffect,
            StackableIsASecondaryContextStackableTarget => secondary?.StackableTargets?.Any(s => s == context.stackableCause) ?? false,
            NoStackable => context.stackableCause == null,

            MainCardAfterFurtherFromSourceThanBefore
                => ThisCard.DistanceTo(context.MainCardInfoAfter.Position) > ThisCard.DistanceTo(context.mainCardInfoBefore.Position),

            MainCardIsSecondaryDelayedCardTarget => context.mainCardInfoBefore.Card == secondary?.DelayedCardTarget,
            SpaceIsSecondaryDelayedSpaceTarget => context.space == secondary?.DelayedSpaceTarget,
            StackableIsSecondaryDelayedStackableTarget => context.stackableCause == secondary?.DelayedStackableTarget,

            //other non-card triggering things
            SpaceFitsRestriction => context.space != null && spaceRestriction.IsValidSpace(context.space, context),

            XFitsRestriction => context.x.HasValue && xRestriction.IsValidNumber(context.x.Value),
            StackableSourceIsMainCard => context.stackableCause is Effect eff && eff.Source == context.mainCardInfoBefore.Card,

            ControllerTriggered => context.player == ThisCard.Controller,
            EnemyTriggered => context.player != ThisCard.Controller,

            //gamestate
            FriendlyTurn => Game.TurnPlayer == ThisCard.Controller,
            EnemyTurn => Game.TurnPlayer != ThisCard.Controller,
            FromField => context.mainCardInfoBefore.Location == CardLocation.Board,
            FromDeck => context.mainCardInfoBefore.Location == CardLocation.Deck,
            NotFromEffect => !(context.stackableCause is Effect),
            FromAttack => context.stackableCause is Attack,

            //max
            MaxPerRound => ThisTrigger.Effect.TimesUsedThisRound < maxPerRound,
            MaxPerTurn => ThisTrigger.Effect.TimesUsedThisTurn < maxTimesPerTurn,
            MaxPerStack => ThisTrigger.Effect.TimesUsedThisStack < maxPerStack,

            //misc
            _ => throw new ArgumentException($"Invalid trigger restriction {restriction}"),
        };


        private bool IsRestrictionValidDebug(string r, ActivationContext ctxt, ActivationContext secondary)
        {
            var success = IsRestrictionValid(r, ctxt, secondary);
            //TODO: tie this to a compiler flag/ifdef sort of thing
            //if (!success) Debug.Log($"Trigger for {ThisCard.CardName} invalid at restriction {r} for {ctxt}");
            return success;
        }

        /// <summary>
        /// Checks whether this trigger restriction is valid for the given context where the trigger occurred.
        /// Can optionally be triggered w/r/t a secondary activation context, for various reasons. See <paramref name="secondary"/>
        /// </summary>
        /// <param name="context">The activation context to evaluate this trigger restriction for</param>
        /// <param name="secondary">A secondary piece of context, like what the activation context was when a hanging effect was applied.</param>
        /// <returns></returns>
        public bool IsValidTriggeringContext(ActivationContext context, ActivationContext secondary = default)
        {
            if (!initialized) throw new ArgumentException("Trigger restriction not initialized!");

            try
            {
                return triggerRestrictions.All(r => IsRestrictionValidDebug(r, context, secondary: secondary))
                    && triggerRestrictionElements.All(tre => tre.IsValidContext(context, secondaryContext: secondary));
            }
            catch (NullReferenceException nullref)
            {
                Debug.LogError($"Trigger restriction of {ThisCard?.CardName} threw a null ref.\n{nullref.Message}\n{nullref.StackTrace}." +
                    $"game was {Game}, this card was {ThisCard}");
                return false;
            }
            catch (ArgumentException argEx)
            {
                Debug.LogError($"Trigger restriction of {ThisCard?.CardName} hit arg ex.\n{argEx.Message}\n{argEx.StackTrace}." +
                    $"game was {Game}, this card was {ThisCard}");
                return false;
            }
        }

        /// <summary>
        /// Reevaluates the trigger to check that any restrictions that could change between it being triggered
        /// and it being ordered on the stack, are still true.
        /// </summary>
        /// <returns></returns>
        public bool IsStillValidTriggeringContext(ActivationContext context)
            => ReevalationRestrictions.Intersect(triggerRestrictions).All(r => IsRestrictionValid(r, context));
    }
}