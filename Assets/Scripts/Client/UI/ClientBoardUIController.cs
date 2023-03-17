using KompasClient.Cards;
using KompasClient.GameCore;
using KompasCore.Cards;
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

        public override void Clicked(Space position)
        {
            if (clientUIController.TargetMode == TargetMode.SpaceTarget)
            {
                var (x, y) = position;
                clientBoardController.clientGame.clientNotifier.RequestSpaceTarget(x, y);
                return;
            }

            var card = UIController.CardViewController.ShownCard as GameCard;
            Debug.Log($"Card {card?.CardName} was selected while clicking on space {position}");
            if (card != null) clientBoardController.AttemptPutCard(card, position);
            //regardless, select nothing
        }
    }
}