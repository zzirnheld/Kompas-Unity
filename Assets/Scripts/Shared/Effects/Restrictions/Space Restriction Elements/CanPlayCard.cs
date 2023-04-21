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

		public bool normalPlay = false;

		public string[] ignoring;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			toPlay.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Space space, IResolutionContext context)
		{
			var restriction = toPlay.From(context, default).PlayRestriction;
		
			return normalPlay
				? restriction.IsValidNormalPlay(space, InitializationContext.Controller, ignoring: ignoring)
				: restriction.IsValidEffectPlay(space, InitializationContext.effect, InitializationContext.Controller, context, ignoring: ignoring);
		}
	}
}