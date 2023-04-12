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
        public const string Unique = "Unique";
        #endregion restrictions

        public string[] cardRestrictions = { };

        public string nameIs;

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
                Unique => potentialTarget?.Unique ?? false,

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