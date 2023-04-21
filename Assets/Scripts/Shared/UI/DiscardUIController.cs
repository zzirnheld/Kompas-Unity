using KompasCore.GameCore;

namespace KompasCore.UI
{
	public class DiscardUIController : StackableGameLocationUIController
	{
		public DiscardController discardController;

		protected override IGameLocation GameLocation => discardController;
	}
}