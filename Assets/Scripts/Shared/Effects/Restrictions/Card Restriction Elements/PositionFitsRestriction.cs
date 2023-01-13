using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class PositionFitsRestriction : CardRestrictionElement
    {
        public SpaceRestriction spaceRestriction;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            spaceRestriction.Initialize(initializationContext);
        }

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => spaceRestriction.IsValidSpace(card.Position, context);
    }

    /// <summary>
    /// Helper class for checking whether a card is adjacent to another card.
    /// For simplicity reasons, default checks whether the card is adjacent to this one.
    /// </summary>
    public class AdjacentTo : PositionFitsRestriction
    {
        public INoActivationContextIdentity<GameCardBase> adjacentTo = new Identities.GamestateCardIdentities.ThisCard();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            spaceRestriction = new SpaceRestriction(){
                spaceRestrictionElements = { new SpaceRestrictionElements.AdjacentTo() { card = adjacentTo } }
            };
            base.Initialize(initializationContext);
        }

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => adjacentTo.Item.IsAdjacentTo(card);
    }
}