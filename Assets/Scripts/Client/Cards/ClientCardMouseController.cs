using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;
using UnityEngine.EventSystems;

namespace KompasClient.Cards
{
    public class ClientCardMouseController : CardMouseController
    {
        //constants
        //minimum and maximum distances to the board, discard, and deck objects for dragging
        public const float minBoardLocalX = -7f;
        public const float maxBoardLocalX = 7f;
        public const float minBoardLocalY = -7f;
        public const float maxBoardLocalY = 7f;
        public const float minDiscardX = 4.5f;
        public const float maxDiscardX = 5.5f;
        public const float minDiscardZ = -3.5f;
        public const float maxDiscardZ = -2.5f;
        public const float minDeckX = 2.5f;
        public const float maxDeckX = 3.5f;
        public const float minDeckZ = -5.5f;
        public const float maxDeckZ = -4.5f;

        public ClientGame ClientGame;
        public override Game Game => ClientGame;

        //helper methods
        public bool WithinIgnoreY(Vector3 position, float minX, float maxX, float minZ, float maxZ)
        {
            return position.x > minX && position.x < maxX && position.z > minZ && position.z < maxZ;
        }
        public bool WithinIgnoreZ(Vector3 position, float minX, float maxX, float minY, float maxY)
        {
            return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
        }

        public override void OnMouseDrag()
        {
            base.OnMouseDrag();
            ClientGame.MarkCardDirty(Card);
        }

        public override void OnMouseExit()
        {
            //remove thing even if hovering over something.
            ClientGame.clientUICtrl.CardToActivateEffectsFor = null;
            base.OnMouseExit();
        }

        public override void OnMouseOver()
        {
            //reset it every time in case exit reset it after on mouse enter would have set it.
            //it's just a memory access per frame. it should be fine.
            //if it leads to counterintuitive behavior, add the is over game object check
            ClientGame.clientUICtrl.CardToActivateEffectsFor = Card;
            /*if(Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                ClientGame.clientUICtrl.cardInfoViewUICtrl.CurrShown = Card;
            }*/
            ClientGame.clientUICtrl.cardInfoViewUICtrl.CurrShown = Card;
            base.OnMouseOver();
        }

        public override void OnMouseUp()
        {
            ClientGame.MarkCardDirty(Card);
            //don't do anything if we're over an event system object, 
            //because that would let us click on cards underneath prompts
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log($"Released card while pointer over event system object");
                return;
            }

            base.OnMouseUp();

            //don't allow dragging cards if we're awaiting a target
            if (Card.Game.targetMode != Game.TargetMode.Free)
            {
                Card.PutBack();
                return;
            }

            //get coords w/r/t gameboard
            var boardLocalPosition = Game.boardObject.transform.InverseTransformPoint(Card.gameObject.transform.position);

            //then, check if it's on the board, accodring to the local coordinates of the game board)
            if (WithinIgnoreZ(boardLocalPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
            {
                int x = BoardController.PosToGridIndex(boardLocalPosition.x);
                int y = BoardController.PosToGridIndex(boardLocalPosition.z);

                //if the card is being moved on the field, that means it's just being moved
                if (Card.Location == CardLocation.Field)
                {
                    var cardThere = Game.boardCtrl.GetCardAt(x, y);
                    // Debug.Log($"Trying to move/attack to {x}, {y}. The controller index, if any, is {(cardThere == null ? -1 : cardThere.ControllerIndex)}");
                    //then check if it's an attack or not
                    if (cardThere != null && cardThere.Controller != Card.Controller)
                        ClientGame.clientNotifier.RequestAttack(Card, cardThere);
                    else
                        ClientGame.clientNotifier.RequestMove(Card, x, y);
                }
                //otherwise, it is being played from somewhere like the hand or discard
                else ClientGame.clientNotifier.RequestPlay(Card, x, y);
            }
            else Debug.Log($"Card {Card.CardName} dragged to somewhere off the board, board local pos {boardLocalPosition}. Only putting back.");

            //regardless, put the card where it goes until we know where to properly put it
            Card.PutBack();
        }
    }
}