using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.PlayerRestrictionElements
{
	public class PlayersMatch : PlayerRestrictionElement
	{
		public IIdentity<Player> player;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			player.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(Player item, IResolutionContext context)
		 => item == player.From(context);
	}
}