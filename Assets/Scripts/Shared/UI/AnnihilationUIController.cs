using KompasCore.GameCore;

namespace KompasCore.UI
{
    public class AnnihilationUIController : StackableGameLocationUIController
    {
        public AnnihilationController annihilationController;

        protected override IGameLocation GameLocation => annihilationController;

        protected override int WrapLen(int objCount) => int.MaxValue;
    }
}