using UnityEngine;
using KompasCore.UI;
using System.Linq;

namespace KompasCore.Cards
{
    [RequireComponent(typeof(GameCardViewController))]
    /// <summary>
    /// Controls the card's physical behavior.
    /// </summary>
    public abstract class CardController : MonoBehaviour
    {
        public GameCardViewController gameCardViewController;

        public abstract GameCard Card { get; }

        public virtual void SetPhysicalLocation(CardLocation location)
        {
            //Debug.Log($"Card controller of {card.CardName} setting physical location in {card.Location} to {card.BoardX}, {card.BoardY}");

            gameCardViewController.Refresh();

            //is the card augmenting something?
            if (Card.Attached)
            {
                gameObject.SetActive(true);
                Card.AugmentedCard.CardController.SpreadOutAugs();
                return;
            }

            //Here on out, we assume the card's not an augment
            Card.transform.localScale = Vector3.one;

            switch (location)
            {
                case CardLocation.Nowhere: break;
                case CardLocation.Deck:
                    Card.gameObject.transform.SetParent(Card.Controller.deckObject.transform);
                    gameObject.SetActive(false);
                    break;
                case CardLocation.Discard:
                    Card.gameObject.transform.SetParent(Card.Controller.discardObject.transform);
                    Card.Controller.discardCtrl.SpreadOutCards();
                    SetRotation();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Board:
                    Card.gameObject.transform.localScale = Vector3.one;
                    Card.gameObject.transform.SetParent(Card.Game.BoardController.gameObject.transform);
                    MoveTo(Card.Position);
                    SetRotation();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Hand:
                    Card.gameObject.transform.SetParent(Card.Controller.handObject.transform);
                    Card.Controller.handCtrl.SpreadOutCards();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Annihilation:
                    Card.gameObject.transform.SetParent(Card.Controller.annihilationCtrl.gameObject.transform);
                    Card.Controller.annihilationCtrl.SpreadOutCards();
                    SetRotation();
                    gameObject.SetActive(true);
                    break;
                default: throw new System.ArgumentException($"Invalid card location {location} to put card physically at");
            }

            SpreadOutAugs();

        }

        /// <summary>
        /// Updates the local position of this card, given a board position
        /// </summary>
        private void MoveTo((int x, int y) to)
        {
            transform.localPosition = BoardUIController.GridIndicesToCardPos(to.x, to.y);
        }

        public void SpreadOutAugs()
        {
            var augCount = Card.Augments.Count();
            float scale = 0.4f; // / ((float)((augCount + 3) / 4));
            int i = 0;
            foreach (var aug in Card.Augments)
            {
                aug.transform.parent = Card.transform;
                aug.transform.localScale = new Vector3(scale, scale, scale);
                float x, z;
                (x, z) = (i % 4) switch
                {
                    0 => (-0.5f, 0.5f),
                    1 => (0.5f, 0.5f),
                    2 => (0.5f, -0.5f),
                    3 => (-0.5f, -0.5f),
                    _ => (0f, 0f),
                };
                aug.transform.localPosition = new Vector3(x, 0.2f * ((i / 4) + 1), z);
                i++;
                aug.CardController.SetRotation();
            }
        }

        public void SetRotation()
        {
            //Debug.Log($"Setting rotation of {Card.CardName}, controlled by {Card.ControllerIndex}, known? {Card.KnownToEnemy}");
            int yRotation = 180 * Card.ControllerIndex;
            int zRotation = 180 * (Card.KnownToEnemy ? 0 : Card.ControllerIndex);
            Card.transform.eulerAngles = new Vector3(0, yRotation, zRotation);
        }

        public void PutBack() => SetPhysicalLocation(Card.Location);
    }
}