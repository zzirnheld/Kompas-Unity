using KompasCore.Cards;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasClient.GameCore
{
    public class ClientBoardController : BoardController
    {
        public ClientGame clientGame;
        public override Game Game => clientGame;

        /// <summary>
        /// The player has just attempted to put a card into a given space.
        /// Request the appropriate actions of the server
        /// </summary>
        /// <param name="card"></param>
        /// <param name="space"></param>
        public void AttemptPutCard(GameCard card, Space space)
        {
            var (x, y) = space;

            //if the card is being moved on the field, that means it's just being moved
            if (card.Location == CardLocation.Board)
            {
                var cardThere = GetCardAt(space);
                Debug.Log($"Trying to move/attack {card} to {x}, {y}. The controller index, if any, is {(cardThere == null ? -1 : cardThere.ControllerIndex)}, compared to that card's {card?.ControllerIndex}");
                //then check if it's an attack or not
                if (cardThere != null && cardThere.Controller != card.Controller)
                    clientGame.clientNotifier.RequestAttack(card, cardThere);
                else
                    clientGame.clientNotifier.RequestMove(card, x, y);
            }
            //otherwise, it is being played from somewhere like the hand or discard
            else clientGame.clientNotifier.RequestPlay(card, x, y);
        }
    }
}