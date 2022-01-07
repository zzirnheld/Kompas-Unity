using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class TriggerRestriction
    {
        public Game Game { get; private set; }

        #region trigger restrictions
        private const string ThisCardTriggered = "This Card Triggered"; //0,
        private const string ThisCardIsSecondaryTriggerer = "This Card is Secondary Triggerer";

        //todo deprecate this card in play to this card fits restriction
        private const string ThisCardInPlay = "This Card in Play"; //1,
        private const string AugmentedCardTriggered = "Augmented Card Triggered"; //10,

        private const string ThisCardFitsRestriction = "This Card Fits Restriction"; //100,
        private const string TriggererFitsRestriction = "Triggerer Fits Restriction"; //101,
        private const string SecondaryCardFitsRestriction = "Secondary Card Fits Restriction";
        private const string TriggererNowFitsRestirction = "Triggerer Now Fits Restriction";
        private const string TriggerersAugmentedCardFitsRestriction = "Triggerer's Augmented Card Fits Restriction";
        private const string CardExists = "Card Exists";
        private const string TriggererIsSecondaryContextTarget = "Triggerer is Secondary Context Target";

        private const string CardNowFurtherFromSourceThanItWas = "Card is now Further from Source than it Was";

        private const string AdjacentToRestriction = "Adjacent to Restriction";
        private const string TargetAdjacentToRestriction = "Target is Adjacent to Restriction";
        private const string CoordsFitRestriction = "Coords Fit Restriction"; //120,
        private const string XFitsRestriction = "X Fits Restriction"; //130,
        private const string StackableSourceFitsRestriction = "Stackable Source Fits Restriction";
        private const string EffectSourceIsThisCard = "Stackable Source is This Card"; //140,
        private const string EffectSourceIsTriggerer = "Stackable Source is Triggerer"; //149,
        private const string StackableSourceNotThisEffect = "Stackable Source isn't This Effect";
        private const string NoStackable = "No Stackable";

        private const string NumberOfCardsFittingRestrictionFitsXRestriction = "Number of Cards Fitting Restriction Fits X Restriction";

        private const string ContextsStackablesMatch = "Contexts Stackables Match";
        private const string StackableIsThisEffect = "Stackable is This Effect";

        private const string DistanceTriggererToSpaceConstant = "Distance from Triggerer to Space == Constant";

        private const string ControllerTriggered = "Controller Triggered"; //200,
        private const string EnemyTriggered = "Enemy Triggered"; //201,

        /* note: turns are the exception to the rule in that they are the only triggers where the triggering event
        * (in their case, the turn passing)
        * happens before the trigger is called, 
        * instead of the trigger being called at the moment before it happens.
        * in short, note that the turn will pass, then the trigger for turn start is called.
        * this means that checking for friendly/enemy turn will check whose turn the current (just-changed-to) turn is. *
        */
        private const string FriendlyTurn = "Friendly Turn"; //300,
        private const string EnemyTurn = "Enemy Turn"; //301,

        private const string FromField = "From Field"; //400,
        private const string FromDeck = "From Deck"; //401,

        private const string MaxPerTurn = "Max Per Turn"; //500,
        private const string NotFromEffect = "Not From Effect"; //501,
        private const string MaxPerRound = "Max Per Round"; //502
        private const string MaxPerStack = "Max Per Stack";
        #endregion trigger conditions

        private static readonly string[] ReevalationRestrictions = { MaxPerTurn, MaxPerRound, MaxPerStack };

        public static readonly string[] DefaultFallOffRestrictions = { ThisCardTriggered, ThisCardInPlay };

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction;
        public CardRestriction nowRestriction;
        public CardRestriction adjacencyRestriction;
        public CardRestriction existsRestriction;
        public XRestriction xRestriction;
        public SpaceRestriction spaceRestriction;
        public CardRestriction sourceRestriction;

        public CardRestriction cardsFittingCardRestriction;
        public XRestriction cardsFittingXRestriction;

        public int maxTimesPerTurn = 1;
        public int maxPerRound = 1;
        public int maxPerStack = 1;
        public int distance = 1;

        public GameCard ThisCard { get; private set; }

        public Trigger ThisTrigger { get; private set; }
        public Effect SourceEffect { get; private set; }
        public Subeffect Subeffect { get; private set; }

        // Necessary because json doesn't let you have nice things, like constructors with arguments,
        // so I need to make sure manually that I've bothered to set up relevant arguments.
        private bool initialized = false;

        public void Initialize(Game game, GameCard thisCard, Trigger thisTrigger, Effect effect, Subeffect subeff = default)
        {
            Game = game;
            ThisCard = thisCard;
            ThisTrigger = thisTrigger;
            SourceEffect = effect;

            if (subeff == default)
            {
                cardRestriction?.Initialize(thisCard, effect);
                existsRestriction?.Initialize(thisCard, effect);
                nowRestriction?.Initialize(thisCard, effect);
                spaceRestriction?.Initialize(thisCard, thisCard.Controller, effect);
                sourceRestriction?.Initialize(thisCard, effect);
            }
            else
            {
                Subeffect = subeff;
                cardRestriction?.Initialize(subeff);
                existsRestriction?.Initialize(subeff);
                nowRestriction?.Initialize(subeff);
                spaceRestriction?.Initialize(subeff);
                sourceRestriction?.Initialize(subeff);
            }
            xRestriction?.Initialize(thisCard);

            initialized = true;
            //Debug.Log($"Initializing trigger restriction for {thisCard?.CardName}. game is null? {game}");
        }


        private bool RestrictionValid(string restriction, ActivationContext context, ActivationContext secondary = default)
        {
            return restriction switch
            {
                //card triggering stuff
                ThisCardTriggered            => context.mainCardInfoBefore.Card == ThisCard,
                ThisCardIsSecondaryTriggerer => context.secondaryCardInfoBefore.Card == ThisCard,
                AugmentedCardTriggered       => context.mainCardInfoBefore.Augments.Contains(ThisCard),

                ThisCardInPlay => ThisCard.Location == CardLocation.Field,
                CardExists     => ThisCard.Game.Cards.Any(c => existsRestriction.Evaluate(c, context)),

                ThisCardFitsRestriction => cardRestriction.Evaluate(ThisCard, context),

                TriggererFitsRestriction    => cardRestriction.Evaluate(context.mainCardInfoBefore, context),
                SecondaryCardFitsRestriction => cardRestriction.Evaluate(context.secondaryCardInfoBefore, context),
                TriggererNowFitsRestirction => nowRestriction.Evaluate(context.MainCardInfoAfter, context),
                TriggerersAugmentedCardFitsRestriction  => cardRestriction.Evaluate(context.mainCardInfoBefore.AugmentedCard, context),
                TriggererIsSecondaryContextTarget       => secondary?.Targets?.Any(c => c == context.mainCardInfoBefore?.Card) ?? false,

                StackableSourceFitsRestriction  => sourceRestriction.Evaluate(context.stackable?.Source, context),
                EffectSourceIsThisCard => context.stackable?.Source == ThisCard,
                StackableSourceNotThisEffect    => context.stackable != SourceEffect,
                ContextsStackablesMatch         => context.stackable == secondary?.stackable,
                StackableIsThisEffect           => context.stackable == SourceEffect,
                NoStackable                     => context.stackable == null,

                CardNowFurtherFromSourceThanItWas => ThisCard.DistanceTo(context.mainCardInfoBefore.Card.Position) > ThisCard.DistanceTo(context.mainCardInfoBefore.Position),
                
                //other non-card triggering things
                CoordsFitRestriction  => context.space != null && spaceRestriction.Evaluate(context.space, context),
                AdjacentToRestriction => ThisCard.AdjacentCards.Any(c => cardRestriction.Evaluate(c, context)),

                XFitsRestriction        => context.x != null && xRestriction.Evaluate(context.x.Value),
                EffectSourceIsTriggerer => context.stackable is Effect eff && eff.Source == context.mainCardInfoBefore.Card,

                //TODO make these into just something to do with triggered card fitting restriction
                ControllerTriggered => context.player == ThisCard.Controller,
                EnemyTriggered      => context.player != ThisCard.Controller,

                //gamestate
                FriendlyTurn    => Game.TurnPlayer == ThisCard.Controller,
                EnemyTurn       => Game.TurnPlayer != ThisCard.Controller,
                FromField       => context.mainCardInfoBefore.Location == CardLocation.Field,
                FromDeck        => context.mainCardInfoBefore.Location == CardLocation.Deck,
                NotFromEffect   => context.stackable is Effect,

                //max
                MaxPerRound => ThisTrigger.Effect.TimesUsedThisRound < maxPerRound,
                MaxPerTurn  => ThisTrigger.Effect.TimesUsedThisTurn < maxTimesPerTurn,
                MaxPerStack => ThisTrigger.Effect.TimesUsedThisStack < maxPerStack,

                //misc
                _ => throw new ArgumentException($"Invalid trigger restriction {restriction}"),
            };
        }

        
        private bool RestrictionValidDebug(string r, ActivationContext ctxt, ActivationContext secondary)
        {
            var success = RestrictionValid(r, ctxt, secondary);
            if (!success) Debug.Log($"Trigger for {ThisCard.CardName} invalid at restriction {r} for {ctxt}");
            return success;
        }

        /// <summary>
        /// Checks whether this trigger restriction is valid for the given context where the trigger occurred.
        /// Can optionally be triggered w/r/t a secondary activation context, for various reasons. See <paramref name="secondary"/>
        /// </summary>
        /// <param name="context">The activation context to evaluate this trigger restriction for</param>
        /// <param name="secondary">A secondary piece of context, like what the activation context was when a hanging effect was applied.</param>
        /// <returns></returns>
        public bool Evaluate(ActivationContext context, ActivationContext secondary = default)
        {
            if (!initialized) throw new ArgumentException("Trigger restriction not initialized!");

            try
            {
                return triggerRestrictions.All(r => RestrictionValidDebug(r, context, secondary: secondary));
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
        public bool Reevaluate(ActivationContext context)
            => triggerRestrictions.Intersect(ReevalationRestrictions).All(r => RestrictionValid(r, context));
    }
}