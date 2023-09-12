using Newtonsoft.Json;

namespace KompasCore.Effects.Identities.Stackables
{
	public class StackableIndex : EffectContextualLeafIdentityBase<IStackable>
	{
		[JsonProperty]
		public int index = -1;

		protected override IStackable AbstractItemFrom(IResolutionContext toConsider)
			=> EffectHelpers.GetItem(toConsider.StackableTargets, index); 
	}
}