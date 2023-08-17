using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Spaces
{
	public class Constant : ContextlessLeafIdentityBase<Space>
	{
		[JsonProperty(Required = Required.Always)]
		public int x;
		[JsonProperty(Required = Required.Always)]
		public int y;

		protected override Space AbstractItem => (x, y);
	}
}