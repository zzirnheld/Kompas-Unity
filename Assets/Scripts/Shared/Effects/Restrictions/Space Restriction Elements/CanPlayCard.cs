using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.SpaceRestrictionElements
{
	/// <summary>
	/// Whether a card can be moved to that space. Presumes from effect
	/// </summary>
	public class CanPlayCard : SpaceRestrictionElement
	{
		public IIdentity<GameCardBase> toPlay;

		public bool ignoreAdjacency;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toPlay.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var restriction = toPlay.From(context).PlayRestriction;
		
			return ignoreAdjacency
				? restriction.IsValidIgnoringAdjacency((space, InitializationContext.Controller), context)
				: restriction.IsValid((space, InitializationContext.Controller), context);
		}
	}
}