using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentImageController : MonoBehaviour, IPointerEnterHandler
{
    private UIController uiCtrl;
    private GameCard card;

    public Image img;

    public void Initialize(GameCard card, UIController uiCtrl)
    {
        this.card = card;
        this.uiCtrl = uiCtrl;
        img.sprite = card.simpleSprite;
    }

    public void OnPointerEnter(PointerEventData eventData) => uiCtrl.SelectCard(card, false);
}
