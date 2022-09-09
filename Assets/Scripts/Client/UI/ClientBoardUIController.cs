using KompasClient.Cards;
using KompasClient.GameCore;
using KompasCore.GameCore;
using KompasCore.UI;
using UnityEngine;

namespace KompasClient.UI
{
    public class ClientBoardUIController : BoardUIController
    {
        public const float minBoardLocalX = -7f;
        public const float maxBoardLocalX = 7f;
        public const float minBoardLocalY = -7f;
        public const float maxBoardLocalY = 7f;

        public ClientUIController clientUIController;
        public ClientBoardController clientBoardController;

        public override UIController UIController => clientUIController;
        public override BoardController BoardController => clientBoardController;

        public override void OnMouseDown()
        {
            base.OnMouseDown();

            if (clientUIController.TargetMode != TargetMode.SpaceTarget) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var intersection = transform.InverseTransformPoint(hit.point);

                int xIntersection = PosToGridIndex(intersection.x);
                int yIntersection = PosToGridIndex(intersection.z);
                //then, if the game is a clientgame, request a space target
                clientBoardController.Game.OnClickBoard(xIntersection, yIntersection);
            }
        }

        public bool CardDragEnded(ClientCardController cardController)
        {
            //get coords w/r/t gameboard
            var boardLocalPosition = gameObject.transform.InverseTransformPoint(cardController.gameObject.transform.position);

            //then, check if it's on the board, accodring to the local coordinates of the game board)
            if (WithinIgnoreZ(boardLocalPosition, minBoardLocalX, maxBoardLocalX, minBoardLocalY, maxBoardLocalY))
            {
                int x = PosToGridIndex(boardLocalPosition.x);
                int y = PosToGridIndex(boardLocalPosition.z);

                clientBoardController.AttemptPutCard(cardController.Card, (x, y));

                return true;
            }
            else Debug.Log($"Card {cardController.Card.CardName} dragged to somewhere off the board, board local pos {boardLocalPosition}. Only putting back.");

            //regardless, put the card where it goes until we know where to properly put it
            cardController.PutBack();
            return false;
        }

        public bool WithinIgnoreZ(Vector3 position, float minX, float maxX, float minY, float maxY)
        {
            return position.x > minX && position.x < maxX && position.y > minY && position.y < maxY;
        }
    }
}