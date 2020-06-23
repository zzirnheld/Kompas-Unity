using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AugmentImageController : MonoBehaviour, IPointerEnterHandler
{
    private UIController uiCtrl;
    private Card card;

    public Image img;

    public void Initialize(Card card, UIController uiCtrl)
    {
        this.card = card;
        this.uiCtrl = uiCtrl;
        img.sprite = card.simpleSprite;
    }

    public void OnPointerEnter(PointerEventData eventData) => uiCtrl.SelectCard(card, false);
}
