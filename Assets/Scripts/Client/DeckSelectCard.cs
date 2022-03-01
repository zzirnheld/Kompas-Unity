﻿using KompasCore.Cards;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace KompasClient.UI
{
    public class DeckSelectCard : CardBase, IPointerDownHandler
    {
        public DeckSelectUIController UICtrl;
        public Image Image;

        public void SetInfo(SerializableCard card, DeckSelectUIController uiCtrl)
        {
            base.SetCardInformation(card);
            UICtrl = uiCtrl;
            SetImage(CardName);
        }

        protected void SetImage(string cardFileName)
        {
            simpleSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
            Image.sprite = simpleSprite;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UICtrl.SelectAsAvatar(this);
        }
    }
}
