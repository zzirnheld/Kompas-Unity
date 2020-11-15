using KompasCore.GameCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasCore.Cards
{
    public abstract class CardMouseController : MonoBehaviour
    {
        public GameCard Card;
        public abstract Game Game { get; }
        protected bool dragging = false;

        #region MouseStuff
        private void GoToMouse()
        {
            //raycast to get point to drag to
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Card.gameObject.transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
            }
        }

        //actual interaction
        public void OnMouseDrag()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            //don't allow dragging cards if we're awaiting a target
            if (Game.targetMode != Game.TargetMode.Free) return;

            dragging = true;
            GoToMouse();
        }

        public virtual void OnMouseExit()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            //don't allow dragging cards if we're awaiting a target
            if (Game.targetMode != Game.TargetMode.Free) return;

            bool mouseDown = Input.GetMouseButton(0);
            if (dragging)
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
            Debug.Log($"On mouse up on {Card.CardName} in target mode {Game.targetMode} at {transform.position}");

            //select cards if the player releases the mouse button while over one
            Game.uiCtrl.SelectCard(Card, true);

            if (!dragging) return;
            dragging = false;
        }

        public virtual void OnMouseOver()
        {
            //if the mouse is currently over a ui element, don't swap what you're seeing
            if (EventSystem.current.IsPointerOverGameObject()) return;
            Game.uiCtrl.HoverOver(Card);
        }
        #endregion MouseStuff
    }
}