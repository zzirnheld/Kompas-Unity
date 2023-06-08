using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class PlayersMatch : TriggerRestrictionBase
	{
		public IIdentity<Player> firstPlayer;
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