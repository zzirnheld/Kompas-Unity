using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the card's physical behavior.
/// </summary>
public class CardController : MonoBehaviour
{
    public GameCard card;
    public MeshRenderer cardFaceRenderer;

    public void SetPhysicalLocation(CardLocation location)
    {
        Debug.Log($"Card controller of {card.CardName} setting physical location in {card.Location} to {card.BoardX}, {card.BoardY}");
        switch (location)
        {
            case CardLocation.Deck:
                card.gameObject.transform.SetParent(card.Controller.deckObject.transform);
                gameObject.SetActive(false);
                break;
            case CardLocation.Discard:
                card.gameObject.transform.SetParent(card.Controller.discardObject.transform);
                card.gameObject.transform.localPosition = new Vector3(0, 0, (float)card.Controller.discardCtrl.IndexOf(card));
                gameObject.SetActive(true);
                break;
            case CardLocation.Field:
                card.gameObject.transform.SetParent(card.Game.boardObject.transform);
                MoveTo((card.BoardX, card.BoardY));
                SetRotation();
                //Card game object active-ness is set in moveTo
                break;
            case CardLocation.Hand:
                card.gameObject.transform.SetParent(card.Controller.handObject.transform);
                card.Controller.handCtrl.SpreadOutCards();
                gameObject.SetActive(true);
                break;
        }
    }

    public void SetRotation()
    {
        card.gameObject.transform.eulerAngles = new Vector3(0, 180 + 180 * card.ControllerIndex, 0);
    }

    /// <summary>
    /// Set the sprites of this card and gameobject
    /// </summary>
    public void SetImage(string cardFileName)
    {
        var detailed = Resources.Load<Texture>("Card Detailed Textures/" + cardFileName);
        //check if either is null. if so, log to debug and return
        if (detailed == null)
        {
            Debug.Log("Could not find sprite with name " + cardFileName);
            return;
        }

        cardFaceRenderer.material.mainTexture = detailed;
    }

    /// <summary>
    /// Sets this card's x and y values and updates its transform
    /// </summary>
    private void MoveTo((int x, int y) to)
    {
        transform.localPosition = BoardController.GridIndicesFromPos(to.x, to.y);
        gameObject.SetActive(card.AugmentedCard == null);
    }
}
