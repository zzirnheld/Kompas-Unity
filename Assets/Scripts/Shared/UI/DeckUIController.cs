using KompasCore.GameCore;

namespace KompasCore.UI
{
    public class DeckUIController : StackableGameLocationUIController
    {
        public DeckController deckController;

        protected override IGameLocation GameLocation => deckController;
    }
}
