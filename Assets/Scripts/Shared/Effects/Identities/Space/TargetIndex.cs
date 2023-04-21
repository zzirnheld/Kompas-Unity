namespace KompasCore.Effects.Identities.Spaces
{
	public class TargetIndex : ContextlessLeafIdentityBase<Space>
	{
		public int index = -1;

		protected override Space AbstractItem => InitializationContext.effect.GetSpace(index);
	}
}