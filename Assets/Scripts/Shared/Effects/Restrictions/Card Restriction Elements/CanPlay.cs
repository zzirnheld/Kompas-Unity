using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
	public class CanPlay : CardRestrictionElement
	{
		public IIdentity<Space> destination;
		public IIdentity<Player> player = new Identities.Players.TargetIndex();
		public string[] ignoring = { };

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			destination?.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
		{
			var controller = player.From(context, default);
			bool IsValidEffectPlay(Space space) => card.PlayRestriction.IsValidEffectPlay(space, InitializationContext.effect, controller, context, ignoring: ignoring);

			if (destination == null) return Space.Spaces.Any(IsValidEffectPlay);
			else return IsValidEffectPlay(destination.From(context, default));
		}
	}
}