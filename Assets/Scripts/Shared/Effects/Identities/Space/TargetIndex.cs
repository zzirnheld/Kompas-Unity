using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Spaces
{
	public class TargetIndex : ContextlessLeafIdentityBase<Space>
	{
		[JsonProperty]
		public int index = -1;

		protected override Space AbstractItem => InitializationContext.effect.GetSpace(index);
	}
}