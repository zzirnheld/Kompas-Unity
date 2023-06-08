namespace KompasCore.Effects.Identities.Stackables
{
	public class ThisEffect : ContextlessLeafIdentityBase<IStackable>
	{
		protected override IStackable AbstractItem => InitializationContext.effect;
	}
}