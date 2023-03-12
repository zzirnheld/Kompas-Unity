using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public abstract class AugmentRestrictionBase : CardRestrictionElement
    {
        public CardRestriction cardRestriction;
        public IIdentity<IReadOnlyCollection<GameCardBase>> augments;
        public IIdentity<GameCardBase> augment;

        private static bool AllNull(params object[] objs) => objs.All(o => o == null);

        protected Func<GameCardBase, bool> IsValidAug(ActivationContext context) => card =>
        {
            if (cardRestriction != null) return cardRestriction.IsValidCard(card, context);
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

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context) 
            => all ? card.Augments.All(IsValidAug(context)) : card.Augments.Any(IsValidAug(context));
    }

    public class Augments : AugmentRestrictionBase
    {
        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => IsValidAug(context)(card.AugmentedCard);
    }
}