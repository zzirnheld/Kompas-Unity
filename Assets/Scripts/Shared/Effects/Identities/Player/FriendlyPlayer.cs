namespace KompasCore.Effects.Identities.Players
{
	public class FriendlyPlayer : ContextlessLeafIdentityBase<Player>
	{
		protected override Player AbstractItem => InitializationContext.Controller;
	}
}
