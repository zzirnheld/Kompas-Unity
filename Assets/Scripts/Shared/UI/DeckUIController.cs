using System.Collections;
using System.Collections.Generic;
using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.UI
{
    public class DeckUIController : StackableGameLocationUIController
    {
        public DeckController deckController;

        protected override IEnumerable<GameCard> Cards => deckController.Cards;
        protected override BaseCardViewController CardViewController => deckController.Game.UIController.CardViewController;
    }
}
