using KompasCore.Effects.Identities;
using Newtonsoft.Json;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class PlayersMatch : TriggerGamestateRestrictionBase
	{
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Player> firstPlayer;
		[JsonProperty(Required = Required.Always)]
		public IIdentity<Player> secondPlayer;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			firstPlayer.Initialize(initializationContext);
			secondPlayer.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> firstPlayer.From(context, secondaryContext) == secondPlayer.From(context, secondaryContext);
	}
}