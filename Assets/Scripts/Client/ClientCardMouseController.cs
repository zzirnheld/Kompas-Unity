using KompasClient.GameCore;
using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;

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

        public override void OnMouseUp()
        {
            base.OnMouseUp();

            //don't allow dragging cards if we're awaiting a target
            if (Card.Game.targetMode != Game.TargetMode.Free) return;

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
                    Debug.Log($"Trying to move/attack to {x}, {y}. The controller index, if any, is {cardThere?.ControllerIndex}");
                    //then check if it's an attack or not
                    if (cardThere != null && cardThere.Controller != Card.Controller)
                        ClientGame.clientNotifier.RequestAttack(Card, x, y);
                    else
                        ClientGame.clientNotifier.RequestMove(Card, x, y);
                }
                //otherwise, it is being played from somewhere like the hand or discard
                else ClientGame.clientNotifier.RequestPlay(Card, x, y);
            }
            //if it's not on the board, maybe it's on top of the discard
            else if (WithinIgnoreY(Card.gameObject.transform.position, minDiscardX, maxDiscardX, minDiscardZ, maxDiscardZ))
            {
                //in that case, discard it //TODO do this by raycasting along another layer to see if you hit deck/discard
                ClientGame.clientNotifier.RequestDiscard(Card);
            }
            //maybe it's on top of the deck
            else if (WithinIgnoreY(Card.gameObject.transform.position, minDeckX, maxDeckX, minDeckZ, maxDeckZ))
            {
                //in that case, topdeck it
                ClientGame.clientNotifier.RequestTopdeck(Card);
            }
            //if it's not in any of those, probably should go back in the hand.
            else
            {
                ClientGame.clientNotifier.RequestRehand(Card);
            }
        }
    }
}