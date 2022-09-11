﻿using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.Cards
{
    //[RequireComponent(typeof(ClientCardController))]
    public class ClientCardMouseController : CardMouseController
    {
        public ClientCardController clientCardController;
        public ClientUIController ClientUIController => clientCardController.ClientUIController;
        public override UIController UIController => ClientUIController;

        public override void OnMouseDrag()
        {
            base.OnMouseDrag();
            clientCardController.ClientGame.MarkCardDirty(card);
        }

        public override void OnMouseExit()
        {
            base.OnMouseExit();
            ClientUIController.cardInfoViewUIController.Refresh();
        }

        public override void OnMouseOver()
        {
            base.OnMouseOver();

            if (Input.GetMouseButtonDown(1)) ClientUIController.rightClickUIController.Show(card);
        }

        public override void OnMouseUp()
        {
            clientCardController.ClientGame.MarkCardDirty(card);
            //don't do anything if we're over an event system object, 
            //because that would let us click on cards underneath prompts
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log($"Released card while pointer over event system object");
                return;
            }

            base.OnMouseUp();

            Debug.Log($"Clicked {card} while target mode is {ClientUIController.TargetMode}");
            //don't allow dragging cards if we're awaiting a target
            if (ClientUIController.TargetMode != TargetMode.Free)
            {
                ClientUIController.clientGame.searchCtrl.ToggleTarget(clientCardController.Card);
                clientCardController.PutBack();
                return;
            }

            ClientUIController.boardUIController.CardDragEnded(clientCardController);
        }
    }
}