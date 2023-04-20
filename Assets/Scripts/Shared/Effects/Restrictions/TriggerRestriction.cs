using KompasCore.Cards;
using KompasCore.Effects.Restrictions;
using KompasCore.Effects.Restrictions.TriggerRestrictionElements;
using KompasCore.GameCore;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects
{
    public class TriggerRestriction : RestrictionBase<TriggeringEventContext>
    {
        //public Game Game { get; private set; }

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
        #endregion trigger conditions
        private static readonly string[] RequiringSelfRestriction = { ThisCardFitsRestriction };
        private static readonly string[] RequiringSourceRestriction = { StackableSourceFitsRestriction };

        public static readonly ISet<Type> ReevalationRestrictions
            = new HashSet<Type>(new Type[] { typeof(MaxPerTurn), typeof(MaxPerRound), typeof(MaxPerStack) });

        public static readonly string[] DefaultFallOffRestrictions = { ThisCardIsMainCard, ThisCardInPlay };

        public string[] triggerRestrictions = new string[0];
        public CardRestriction cardRestriction;
        public CardRestriction selfRestriction;
        public CardRestriction sourceRestriction;

        public IRestriction<TriggeringEventContext>[] triggerRestrictionElements = { };

        private GameCard ThisCard => InitializationContext.source;
        private Game Game => InitializationContext.game;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            cardRestriction?.Initialize(initializationContext);
            selfRestriction?.Initialize(initializationContext);
            sourceRestriction?.Initialize(initializationContext);

            foreach (var tre in triggerRestrictionElements)
            {
                tre.Initialize(initializationContext);
            }

            //Verify that any relevant restrictions exist
            if (triggerRestrictions.Intersect(RequiringCardRestriction).Any() && cardRestriction == null)
                throw new ArgumentNullException("cardRestriction", $"Must be populated for any of these restrictions: {string.Join(",", RequiringCardRestriction)}");
            if (triggerRestrictions.Intersect(RequiringSelfRestriction).Any() && selfRestriction == null)
                throw new ArgumentNullException("selfRestriction", $"Must be populated for any of these restrictions: {string.Join(",", RequiringSelfRestriction)}");
            if (triggerRestrictions.Intersect(RequiringSourceRestriction).Any() && sourceRestriction == null)
                throw new ArgumentNullException("sourceRestriction", $"Must be populated for any of these restrictions: {string.Join(",", RequiringSourceRestriction)}");

            //Debug.Log($"Initializing trigger restriction for {thisCard?.CardName}. game is null? {game}");
        }


        private bool IsRestrictionValid(string restriction, TriggeringEventContext triggeringContext, IResolutionContext stashedResolutionContext = default) => restriction switch
        {
            //card triggering stuff
            ThisCardIsMainCard => triggeringContext.mainCardInfoBefore?.Card == ThisCard,
            ThisCardIsSecondaryCard => triggeringContext.secondaryCardInfoBefore.Card == ThisCard,
            AugmentedCardIsMainCard => triggeringContext.mainCardInfoBefore.Augments.Contains(ThisCard),

            ThisCardInPlay => ThisCard.Location == CardLocation.Board,

            ThisCardFitsRestriction => selfRestriction.IsValid(ThisCard, new ResolutionContext(triggeringContext)),

            MainCardFitsRestrictionBefore => cardRestriction.IsValid(triggeringContext.mainCardInfoBefore, new ResolutionContext(triggeringContext)),
            MainCardsAugmentedCardBeforeFitsRestriction => cardRestriction.IsValid(triggeringContext.mainCardInfoBefore.AugmentedCard, new ResolutionContext(triggeringContext)),

            MainCardIsStackableSource => triggeringContext.stackableCause?.Source == triggeringContext.mainCardInfoBefore.Card,
            StackableSourceFitsRestriction => sourceRestriction.IsValid(triggeringContext.stackableCause?.Source, new ResolutionContext(triggeringContext)),

            //misc
            _ => throw new ArgumentException($"Invalid trigger restriction {restriction}"),
        };

        private static bool StackablesMatch(TriggeringEventContext context, IResolutionContext secondary)
        {
            //Debug.Log($"Comparing {context}'s {context.stackableEvent} and {secondary}'s {secondary?.stackableEvent}");
            return context.stackableEvent == secondary?.TriggerContext.stackableEvent;
        }

        private bool IsRestrictionValidDebug(string r, TriggeringEventContext triggeringContext, IResolutionContext stashedResolutionContext)
        {
            var success = IsRestrictionValid(r, triggeringContext, stashedResolutionContext);
            //TODO: tie this to a compiler flag/ifdef sort of thing
            //if (!success) Debug.Log($"Trigger for {ThisCard.CardName} invalid at restriction {r} for {triggeringContext}");
            return success;
        }

        /// <summary>
        /// Checks whether this trigger restriction is valid for the given context where the trigger occurred.
        /// Can optionally be triggered w/r/t a secondary activation context, for various reasons. See <paramref name="secondary"/>
        /// </summary>
        /// <param name="context">The activation context to evaluate this trigger restriction for</param>
        /// <param name="secondary">A secondary piece of context, like what the activation context was when a hanging effect was applied.</param>
        /// <returns></returns>
        public bool IsValidTriggeringContext(TriggeringEventContext context, IResolutionContext secondary = default)
        {
            ComplainIfNotInitialized();

            try
            {
                return triggerRestrictions.All(r => IsRestrictionValidDebug(r, context, stashedResolutionContext: secondary))
                    && triggerRestrictionElements.All(tre => tre.IsValid(context, secondary));
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
        /// (Not relevant to delayed things, since those expire after a given number of uses (if at all), so yeah
        /// </summary>
        /// <returns></returns>
        public bool IsStillValidTriggeringContext(TriggeringEventContext context)
            => elements.Where(elem => ReevalationRestrictions.Contains(elem.GetType()))
                       .All(elem => elem.IsValid(context, default));
    }
}