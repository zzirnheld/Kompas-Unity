using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasCore.Cards
{
    public abstract class CardMouseController : MonoBehaviour
    {
        [Header("GameCard that this MouseController handles")]
        public CardController card;

        public abstract UIController UIController { get; }

        protected static bool OverActualUIElement => EventSystem.current.IsPointerOverGameObject()
                && EventSystem.current.currentSelectedGameObject != null
                && EventSystem.current.currentSelectedGameObject != HandCameraController.Main.rawImage.gameObject;

        #region MouseStuff
        public virtual void OnMouseExit()
        {
            if (OverActualUIElement) return;

            bool mouseDown = Input.GetMouseButton(0);
            //If the mouse isn't held down rn, then we want to stop showing whatever we're currently showing.
            if (!mouseDown) UIController.CardViewController.Show(null);
        }

        public virtual void OnMouseUp()
        {
            //don't do anything if we're over an event system object, 
            //because that would let us click on cards underneath prompts
            if (OverActualUIElement) return;

            //select cards if the player releases the mouse button while over one
            UIController.CardViewController.Focus(card.Card);
        }

        //TODO factor this out to a card controller base class, then inherit it for the search cards?
        public virtual void OnMouseOver()
        {
            //if the mouse is currently over a ui element, don't swap what you're seeing
            //if (EventSystem.current.IsPointerOverGameObject()) return;

            //TODO still hover over even if mouse is on the effect/attack blocks, lol
            UIController.CardViewController.Show(card.Card);
        }
        #endregion MouseStuff
    }
}