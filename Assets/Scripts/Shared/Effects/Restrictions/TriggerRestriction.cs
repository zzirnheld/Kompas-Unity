using KompasCore.Cards;
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

        public static readonly ISet<Type> ReevalationRestrictions
            = new HashSet<Type>(new Type[] { typeof(MaxPerTurn), typeof(MaxPerRound), typeof(MaxPerStack) });

        public static readonly IRestriction<TriggeringEventContext>[] DefaultFallOffRestrictions = {
            new CardsMatch(){
                card = new Identities.Cards.ThisCard(),
                other = new Identities.Cards.CardBefore()
            },
            new ThisCardInPlay() };

        public IRestriction<TriggeringEventContext>[] triggerRestrictionElements = { };

        private GameCard ThisCard => InitializationContext.source;
        private Game Game => InitializationContext.game;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            foreach (var tre in triggerRestrictionElements)
            {
                tre.Initialize(initializationContext);
            }

            //Debug.Log($"Initializing trigger restriction for {thisCard?.CardName}. game is null? {game}");
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
                return triggerRestrictionElements.All(tre => tre.IsValid(context, secondary));
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