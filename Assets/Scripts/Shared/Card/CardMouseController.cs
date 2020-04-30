using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CardMouseController : MonoBehaviour
{
    public Card Card;
    public abstract Game Game { get; }
    protected bool dragging = false;

    public const float spacesInGrid = 7f;
    public const float boardLenOffset = 0.45f;

    protected static int PosToGridIndex(float pos)
    {
        /*first, add the offset to make the range of values from (-0.45, 0.45) to (0, 0.9).
        * then, multiply by the grid length to board length ratio (currently 7, because there
        * are 7 game board slots for the board's local length of 1). 
        * Divide by 0.9f because the range of accepted position values is 0 to 0.9f (0.45 - -0.45).
        * Then add 0.5 so that the cast to int effectively rounds instead of flooring.
        */
        return (int)(((pos + boardLenOffset) * (spacesInGrid - 1f) / (2 * boardLenOffset)) + 0.5f);
    }

    #region MouseStuff
    private void GoToMouse()
    {
        //raycast to get point to drag to
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = new Vector3(hit.point.x, 1f, hit.point.z);
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
