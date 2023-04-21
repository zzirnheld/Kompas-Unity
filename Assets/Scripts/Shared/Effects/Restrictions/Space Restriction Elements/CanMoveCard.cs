using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Collections.Generic;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
    /// <summary>
    /// Whether a card can be moved to that space. Presumes from effect
    /// </summary>
    public class CanMoveCard : SpaceRestrictionElement
    {
        public IIdentity<GameCardBase> toMove;

        /// <summary>
        /// Describes any restriction on the spaces between the card and where it needs to go (the space being tested)
        /// </summary>
        public IRestriction<Space> throughRestriction = new Empty();

        public NumberRestriction distanceRestriction = new NumberRestriction() {
            numberRestrictions = { }
        };

        public bool normalMove = false;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            toMove.Initialize(initializationContext);
            throughRestriction?.Initialize(initializationContext);
            distanceRestriction?.Initialize(initializationContext);
        }

        private bool FitsMovementRestriction(GameCardBase card, Space space, IResolutionContext context)
            => normalMove 
                ? card.MovementRestriction.IsValidNormalMove(space)
                : card.MovementRestriction.IsValidEffectMove(space, context);

        private bool FitsThroughRestriction(Space source, Space dest, IResolutionContext context)
            => InitializationContext.game.BoardController.AreConnectedByNumberOfSpacesFittingPredicate(source, dest,
                s => throughRestriction.IsValid(s, context), d => distanceRestriction.IsValid(d, context));

        protected override bool IsValidLogic(Space space, IResolutionContext context)
        {
            var card = toMove.From(context, default).Card;
            return FitsMovementRestriction(card, space, context) && FitsThroughRestriction(card.Position, space, context);
        }

    }
}