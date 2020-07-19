using KompasCore.GameCore;
using UnityEngine;

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
            //don't allow dragging cards if we're awaiting a target
            if (Game.targetMode != Game.TargetMode.Free) return;

            dragging = true;
            GoToMouse();
        }

        public void OnMouseExit()
        {
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
            Debug.Log($"On mouse up on {Card.CardName} in target mode {Game.targetMode}");

            //select cards if the player releases the mouse button while over one
            Game.uiCtrl.SelectCard(Card, true);

            if (!dragging) return;
            dragging = false;
        }

        public void OnMouseEnter()
        {
            Game.uiCtrl.HoverOver(Card);
        }
        #endregion MouseStuff
    }
}