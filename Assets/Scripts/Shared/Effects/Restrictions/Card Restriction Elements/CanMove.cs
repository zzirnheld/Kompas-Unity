using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasServer.Effects.Subeffects;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class CanMove : CardRestrictionElement
	{
		public IIdentity<Space> destination;
		/// <summary>
		/// Index of a subeffect whose space restriction should be considered for whether you'll be able to move this card there.
		/// Be warned! If there's any additional targeting in the meantime (on which the valid movement depends) this might not work as expected.
		/// I'll need to figure out a better solution, if one is possible.
		/// </summary>
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

			if (destination != null) return IsValidMoveSpace(destination.From(context));
			else if (spaceRestrictionSubeffectIndex != default)
			{
				if (InitializationContext.effect.Subeffects[spaceRestrictionSubeffectIndex] is not SpaceTarget spaceTargetSubeffect)
				{
					throw new System.ArgumentException($"{spaceRestrictionSubeffectIndex} isn't a space target subeffect! resolving {InitializationContext.effect}");
				}

				bool IsSubeffectRestrictionValid(Space space) => spaceTargetSubeffect.spaceRestriction.IsValid(space, context);
				return InitializationContext.effect.identityOverrides.WithTargetCardOverride(card,
					() => Space.Spaces.Where(IsSubeffectRestrictionValid).Any(IsValidMoveSpace));
			}
			else return Space.Spaces.Any(IsValidMoveSpace);
		}
	}
}