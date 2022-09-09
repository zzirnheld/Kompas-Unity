using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasCore.Cards
{
    public abstract class CardMouseController : MonoBehaviour
    {
        [Header("GameCard that this MouseController handles")]
        public GameCard card;

        public abstract UIController UIController { get; }
        //public abstract Game Game { get; }

        protected bool dragging = false;

        #region MouseStuff
        private void GoToMouse()
        {
            //raycast to get point to drag to
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                card.gameObject.transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
            }
        }

        //actual interaction
        public virtual void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            //don't allow dragging cards if we're awaiting a target
            if (!UIController.AllowDragging) return;

            dragging = true;
            GoToMouse();
        }

        public virtual void OnMouseExit()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            bool mouseDown = Input.GetMouseButton(0);
            //If the mouse isn't held down rn, then we want to stop showing whatever we're currently showing.
            if (!mouseDown && !dragging) UIController.CardViewController.Show(null);

            if (UIController.AllowDragging && dragging)
            {
                if (mouseDown) GoToMouse();
                else OnMouseUp();

                dragging = mouseDown;
            }
        }

        public virtual void OnMouseUp()
        {
            //don't do anything if we're over an event system object, 
            //because that would let us click on cards underneath prompts
            if (EventSystem.current.IsPointerOverGameObject()) return;

            //select cards if the player releases the mouse button while over one
            UIController.CardViewController.Focus(card);

            if (!dragging) return;
            dragging = false;
        }

        public virtual void OnMouseOver()
        {
            //if the mouse is currently over a ui element, don't swap what you're seeing
            if (EventSystem.current.IsPointerOverGameObject()) return;

            //TODO still hover over even if mouse is on the effect/attack blocks, lol
            UIController.CardViewController.Show(card);
        }
        #endregion MouseStuff
    }
}