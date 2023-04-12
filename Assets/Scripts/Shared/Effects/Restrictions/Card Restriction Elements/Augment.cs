using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.elements
{
    //TODO: this can probably be merged with/generalized to a "card is" sort of restriction,
    // where there's an additional IIdentity<GameCardBase> that defines the card to actually be tested in terms of the incoming card?
    public abstract class AugmentRestrictionBase : CardRestrictionElement
    {
        public CardRestriction cardRestriction;
        public IIdentity<IReadOnlyCollection<GameCardBase>> augments;
        public IIdentity<GameCardBase> augment;

        private static bool AllNull(params object[] objs) => objs.All(o => o == null);

        /// <summary>
        /// Returns a predicate that tests the test card with the following order of priorities:
        /// If the cardRestriction is defined, checks that the test card fits that restriction.
        /// If no CardRestriction is defined, but a list of cards is defined, checks if the test card is one of those cards.
        /// If neither is defined, but a single card identity is defined, checks if the test card is that card.
        /// </summary>
        protected Func<GameCardBase, bool> IsValidAug(IResolutionContext context) => card =>
        {
            if (cardRestriction != null) return cardRestriction.IsValid(card, context);
            if (augments != null) return augments.From(context, null).Contains(card);
            if (augment != null) return augment.From(context, null) == card;
            throw new System.ArgumentNullException("augment", $"No augment provided for {this.GetType()} CardRestrictionElement");
        };

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            if (AllNull(cardRestriction, augment, augments))
                throw new System.ArgumentNullException("augment", $"No augment provided for {this.GetType()} CardRestrictionElement");

            cardRestriction?.Initialize(initializationContext);
            augments?.Initialize(initializationContext);
            augment?.Initialize(initializationContext);
        }

    }

    public class HasAugment : AugmentRestrictionBase
    {
        public bool all = false; //default to any

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context) 
            => all
                ? card.Augments.All(IsValidAug(context))
                : card.Augments.Any(IsValidAug(context));
    }

    public class Augments : AugmentRestrictionBase
    {
        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => IsValidAug(context)(card.AugmentedCard);
    }
}