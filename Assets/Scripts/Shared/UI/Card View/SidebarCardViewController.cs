using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    /// <summary>
    /// Controls showing the card in the main, top-left-of-the-board, location.
    /// Also delegates any additional gameplay highlights to the appropriate things
    /// </summary>
    public abstract class SidebarCardViewController : TypicalCardViewController
    {
        [Header("Overall game-related UI controllers")]
        public BoardUIController boardUIController;

        protected override void DisplayNothing()
        {
            base.DisplayNothing();

            //Delegate other responsibilities
            boardUIController.ShowNothing();
        }

        protected override void Display()
        {
            base.Display();

            boardUIController.ShowForCard(ShownCard);
        }
    }
}