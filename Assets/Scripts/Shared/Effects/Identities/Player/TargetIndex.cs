namespace KompasCore.Effects.Identities.Players
{

	public class TargetIndex : ContextlessLeafIdentityBase<Player>
	{
		public int index = -1;

		protected override Player AbstractItem => InitializationContext.effect.GetPlayer(index);
	}
}