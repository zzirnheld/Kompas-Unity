using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;

namespace KompasCore.UI
{
    public class AnnihilationUIController : StackableGameLocationUIController
    {
        public AnnihilationController annihilationController;

        protected override IEnumerable<GameCard> Cards => annihilationController.Cards;
        protected override BaseCardViewController CardViewController => annihilationController.Game.UIController.CardViewController;
    }
}