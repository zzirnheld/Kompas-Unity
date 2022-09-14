using KompasCore.Cards;
using KompasCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.UI.Search
{
    public class SearchCardViewController : GameCardlikeViewController
    {
        public CardController CardController { get; private set; }

        private SearchUIController searchUIController;

        public void Initialize(SearchUIController searchUIController, CardController cardController)
        {
            CardController = cardController;
            Debug.Log($"Showing {CardController.Card} as search card view ctrl");
            Focus(cardController.Card);
            DisplaySpecialEffects();

            this.searchUIController = searchUIController;
        }

        protected override void DisplaySpecialEffects()
        {
            currentTargetObject.SetActive(ShownGameCard.Game.IsCurrentTarget(ShownGameCard));
        }

        public void OnMouseOver()
        {
            //if the mouse is currently over a ui element, don't swap what you're seeing
            //if (EventSystem.current.IsPointerOverGameObject()) return;
            //Debug.Log($"Showing {CardController.Card}");

            //TODO still hover over even if mouse is on the effect/attack blocks, lol
            searchUIController.cardViewController.Show(CardController.Card);
        }

        public void OnMouseUp()
        {
            Debug.Log($"Clicked {CardController.Card} for search");
            searchUIController.OnClick(this);
            DisplaySpecialEffects();
        }
    }
}