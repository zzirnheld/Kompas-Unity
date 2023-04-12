using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasServer.Effects.Subeffects;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class CanMove : CardRestrictionElement
    {
        public IIdentity<Space> destination;
        public int spaceRestrictionSubeffectIndex;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            destination?.Initialize(initializationContext);
            if (spaceRestrictionSubeffectIndex != default
                && InitializationContext.effect.Subeffects[spaceRestrictionSubeffectIndex] is not SpaceTarget)
            {
                throw new System.ArgumentException($"{spaceRestrictionSubeffectIndex} isn't a space target subeffect! for {InitializationContext.effect}");
            }
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        {
            bool IsValidMoveSpace(Space space) => card.MovementRestriction.IsValidEffectMove(space, context);

            if (destination != null) return IsValidMoveSpace(destination.From(context, default));
            else if (spaceRestrictionSubeffectIndex != default)
            {
                if (InitializationContext.effect.Subeffects[spaceRestrictionSubeffectIndex] is not SpaceTarget spaceTargetSubeffect)
                {
                    throw new System.ArgumentException($"{spaceRestrictionSubeffectIndex} isn't a space target subeffect! resolving {InitializationContext.effect}");
                }

                bool IsSubeffectRestrictionValid(Space space) => spaceTargetSubeffect.spaceRestriction.IsValidSpace(space, context);
                return Space.Spaces.Where(IsSubeffectRestrictionValid).Any(IsValidMoveSpace);
            }
            else return Space.Spaces.Any(IsValidMoveSpace);
        }
    }
}