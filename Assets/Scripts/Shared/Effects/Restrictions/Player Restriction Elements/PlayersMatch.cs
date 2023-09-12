using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.PlayerRestrictionElements
{
	public class PlayersMatch : PlayerRestrictionElement
	{
		[JsonProperty(Required = Required.Always)]
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