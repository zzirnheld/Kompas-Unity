
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour {

    //constants
    //all cards should be on layer 9
    public const int layerMask = 1 << 9;

    //raycasting variables
    private Ray mouseRay;
    private RaycastHit raycastHit;
    private Vector3 rayIntersectBoard;
    //game objects hit
    private GameObject gameObjectHit;
    private GameObject lastHit;
    private KompasObject kompasObjectHit;
    private Card cardHit;
    private DeckController deckHit;
    private HandController handHit;

    //raycasting
    public Ray GetMouseRay()
    {
        mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        return mouseRay;
    }
    public RaycastHit GetRaycastHit()
    {
        Physics.Raycast(mouseRay, out raycastHit, float.PositiveInfinity, layerMask);
        return raycastHit;
    }

    //ray intersection
    public Vector3 GetRayIntersectBoard(Ray mouseRay)
    {
        rayIntersectBoard = new Vector3(
            (2f - 1 * mouseRay.origin.y) * mouseRay.direction.x / mouseRay.direction.y,     //x
            2,                                                                              //y. 2 because 2 is where cards should hover
            (2f - 1 * mouseRay.origin.y) * mouseRay.direction.z / mouseRay.direction.y);    //z
        return rayIntersectBoard;
    }
    public Vector3 GetRayIntersectBoard() { return GetRayIntersectBoard(GetMouseRay()); }

    //methods to call during update
    //helper method
    private void ClearHits()
    {
        gameObjectHit = null;
        cardHit = null;
        deckHit = null;
        handHit = null;
    }

    public void DragBeforeRaycast()
    {
        rayIntersectBoard = GetRayIntersectBoard(mouseRay);
        if (Input.GetMouseButton(0)) //ok see if we're dragging something
        {
            if (cardHit != null) //if it hasn't been set to null yet we're still dragging it
            { //ok so we are dragging something
                cardHit.OnDrag(rayIntersectBoard);
                //return;
            }
        }
        else if (Input.GetMouseButtonUp(0)) //maybe we just stopped dragging something
        {
            if (cardHit != null)
            { //ok so we just stopped dragging something
                cardHit.OnDragEnd(rayIntersectBoard);
                cardHit = null;
                //return;
            }
        }
    }

    public void NormalClickObject()
    {
        //to get game object
        if (raycastHit.transform != null) gameObjectHit = raycastHit.transform.gameObject;
        else gameObjectHit = null;

        if (gameObjectHit != null) kompasObjectHit = gameObjectHit.GetComponent<KompasObject>();
        else kompasObjectHit = null;

        if (kompasObjectHit == null) ClearHits();
        else if (kompasObjectHit is Card)
        {
            cardHit = kompasObjectHit as Card;
            if (Input.GetMouseButtonDown(0))
                cardHit.OnClick();
            else if (Input.GetMouseButton(0))
                cardHit.OnDrag(raycastHit.point);
            else if (Input.GetMouseButtonUp(0))
                cardHit.OnDragEnd(raycastHit.point);
        }
        else if (kompasObjectHit is DeckController)
        {
            deckHit = kompasObjectHit as DeckController;
            if (Input.GetMouseButtonDown(0))
                deckHit.OnClick();
        }
        else if (kompasObjectHit is HandController)
        {
            handHit = kompasObjectHit as HandController;
            //this might get clicked if the player misses clicking a card to target. do anything?
        }
        else Debug.Log("IDK what we hit. it wasn't null and it wasn't a card.");
    }


}
