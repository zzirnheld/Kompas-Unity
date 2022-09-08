using KompasClient.GameCore;
using KompasCore.UI;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientGameMainCardViewController : GameMainCardViewController
    {
        public ClientGame clientGame;

        //[Header("Clientside-specific objects")]

        [Header("Other things to display card-related info")]
        public BoardUIController boardUIController;
        public EffectsUIController effectsUIController;

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            ClearShownUniqueCopies();
            ClearShownCardLinks();

            //Delegate other responsibilities
            boardUIController.ShowNothing();
        }

        protected override void Display()
        {
            base.Display();

            //Delegate other responsibilities
            boardUIController.ShowForCard(ShownCard);
            effectsUIController.ShowEffButtons(ShownCard);
        }

        private void ShowPipsAvailableForCost()
        {
            ClientPlayer clientPlayer = ShownCard.Controller as ClientPlayer;
            int costToHighlight = ShownCard.Location == CardLocation.Hand && ShownCard.Cost <= ShownCard.Controller.Pips
                ? ShownCard.Cost
                : 0;
            clientPlayer.pipsUICtrl.HighlightPipsFor(costToHighlight);
        }
    }
}