using KompasCore.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;

namespace KompasCore.UI
{
    public class DiscardUIController : StackableGameLocationUIController
    {
        public DiscardController discardController;

        protected override IEnumerable<GameCard> Cards => discardController.Cards;
        protected override BaseCardViewController CardViewController => discardController.Game.UIController.CardViewController;
    }
}