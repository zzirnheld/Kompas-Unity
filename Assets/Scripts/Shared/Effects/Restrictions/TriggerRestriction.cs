﻿using KompasCore.Cards;
using KompasCore.GameCore;
using System;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class TriggerRestriction
    {
        public Game Game { get; private set; }

        #region trigger conditions
        public const string ThisCardTriggered = "This Card Triggered"; //0,

        //todo deprecate this card in play to this card fits restriction
        public const string ThisCardInPlay = "This Card in Play"; //1,
        public const string AugmentedCardTriggered = "Augmented Card Triggered"; //10,

        public const string ThisCardFitsRestriction = "This Card Fits Restriction"; //100,
        public const string TriggererFitsRestriction = "Triggerer Fits Restriction"; //101,
        public const string TriggererNowFitsRestirction = "Triggerer Now Fits Restriction";
        public const string TriggerersAugmentedCardFitsRestriction = "Triggerer's Augmented Card Fits Restriction";
        public const string CardExists = "Card Exists";
        public const string TriggererIsSecondaryContextTarget = "Triggerer is Secondary Context Target";

        public const string CardNowFurtherFromSourceThanItWas = "Card is now Further from Source than it Was";

        public const string AdjacentToRestriction = "Adjacent to Restriction";
        public const string TargetAdjacentToRestriction = "Target is Adjacent to Restriction";
        public const string CoordsFitRestriction = "Coords Fit Restriction"; //120,
        public const string XFitsRestriction = "X Fits Restriction"; //130,
        public const string StackableSourceFitsRestriction = "Stackable Source Fits Restriction";
        public const string EffectSourceIsThisCard = "Stackable Source is This Card"; //140,
        public const string EffectSourceIsTriggerer = "Stackable Source is Triggerer"; //149,
        public const string StackableSourceNotThisEffect = "Stackable Source isn't This Effect";

        public const string ContextsStackablesMatch = "Contexts Stackables Match";
        public const string StackableIsThisEffect = "Stackable is This Effect";

        public const string DistanceTriggererToSpaceConstant = "Distance from Triggerer to Space == Constant";

        public const string ControllerTriggered = "Controller Triggered"; //200,
        public const string EnemyTriggered = "Enemy Triggered"; //201,

        /* note: turns are the exception to the rule in that they are the only triggers where the triggering event
        * (in their case, the turn passing)
        * happens before the trigger is called, 
        * instead of the trigger being called at the moment before it happens.
        * in short, note that the turn will pass, then the trigger for turn start is called.
        * this means that checking for friendly/enemy turn will check whose turn the current (just-changed-to) turn is. *
        */
        public const string FriendlyTurn = "Friendly Turn"; //300,
        public const string EnemyTurn = "Enemy Turn"; //301,

        public const string FromField = "From Field"; //400,
        public const string FromDeck = "From Deck"; //401,

        public const string MaxPerTurn = "Max Per Turn"; //500,
        public const string NotFromEffect = "Not From Effect"; //501,
        public const string MaxPerRound = "Max Per Round"; //502
        public const string MaxPerStack = "Max Per Stack";
        #endregion trigger conditions

        public static readonly string[] ReevalationRestrictions = { MaxPerTurn, MaxPerRound, MaxPerStack };

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction;
        public CardRestriction nowRestriction;
        public CardRestriction adjacencyRestriction;
        public CardRestriction existsRestriction;
        public XRestriction xRestriction;
        public SpaceRestriction spaceRestriction;
        public CardRestriction sourceRestriction;
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
            switch (restriction)
            {
                //card triggering stuff
                case ThisCardTriggered:        return context.CardInfo.Card == ThisCard;
                case ThisCardInPlay:           return ThisCard.Location == CardLocation.Field;
                case AugmentedCardTriggered:   return context.CardInfo.Augments.Contains(ThisCard);
                case CardExists:               return ThisCard.Game.Cards.Any(c => existsRestriction.Evaluate(c, context));
                case ThisCardFitsRestriction:  return cardRestriction.Evaluate(ThisCard, context);
                case TriggererFitsRestriction: return cardRestriction.Evaluate(context.CardInfo, context);
                case TriggererNowFitsRestirction: return nowRestriction.Evaluate(context.CardInfo.Card, context);
                case TriggerersAugmentedCardFitsRestriction: return cardRestriction.Evaluate(context.CardInfo.AugmentedCard, context);
                case StackableSourceFitsRestriction: return sourceRestriction.Evaluate(context.Stackable?.Source, context);
                case TriggererIsSecondaryContextTarget: return secondary?.Targets?.Any(c => c == context.CardInfo?.Card) ?? false;
                case StackableSourceNotThisEffect: return context.Stackable != SourceEffect;

                case CardNowFurtherFromSourceThanItWas:
                    return ThisCard.DistanceTo(context.CardInfo.Card.Position) > ThisCard.DistanceTo(context.CardInfo.Position); 

                case ContextsStackablesMatch:
                    Debug.Log($"Primary stackable: {context.Stackable}");
                    Debug.Log($"Secondary stackable: {secondary?.Stackable}");
                    Debug.Log($"Equal? {context.Stackable == secondary?.Stackable}");
                    return context.Stackable == secondary?.Stackable;

                case StackableIsThisEffect: return context.Stackable == SourceEffect;

                //other non-card triggering things
                case CoordsFitRestriction:    return context.Space != null && spaceRestriction.Evaluate(context.Space.Value, context);
                case XFitsRestriction:        return context.X != null && xRestriction.Evaluate(context.X.Value);
                case EffectSourceIsTriggerer: return context.Stackable is Effect eff && eff.Source == context.CardInfo.Card;
                case AdjacentToRestriction:
                    Debug.Log($"Card {ThisCard?.CardName} with adjacent cards {ThisCard?.AdjacentCards}");
                    return ThisCard.AdjacentCards.Any(c => cardRestriction.Evaluate(c, context));
                //TODO make these into just something to do with triggered card fitting restriction
                case ControllerTriggered:     return context.Triggerer == ThisCard.Controller;
                case EnemyTriggered:          return context.Triggerer != ThisCard.Controller;

                //gamestate
                case FriendlyTurn:  return Game.TurnPlayer == ThisCard.Controller;
                case EnemyTurn:     return Game.TurnPlayer != ThisCard.Controller;
                case FromField:     return context.CardInfo.Location == CardLocation.Field;
                case FromDeck:      return context.CardInfo.Location == CardLocation.Deck;
                case NotFromEffect: return context.Stackable is Effect;

                //max
                case MaxPerRound: return ThisTrigger.Effect.TimesUsedThisRound < maxPerRound;
                case MaxPerTurn:  return ThisTrigger.Effect.TimesUsedThisTurn < maxTimesPerTurn;
                case MaxPerStack: return ThisTrigger.Effect.TimesUsedThisStack < maxPerStack;

                //misc
                default: throw new ArgumentException($"Invalid trigger restriction {restriction}");
            }
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