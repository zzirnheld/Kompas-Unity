using KompasCore.GameCore;
using UnityEngine;

namespace KompasCore.Cards
{
    /// <summary>
    /// Controls the card's physical behavior.
    /// </summary>
    public class CardController : MonoBehaviour
    {
        public GameCard card;
        public MeshRenderer cardFaceRenderer;
        public GameObject zoomedCharFrame;
        public GameObject zoomedAllFrame;
        public GameObject unzoomedCharFrame;
        public GameObject unzoomedAllFrame;
        public TMPro.TMP_Text zoomedNText;
        public TMPro.TMP_Text zoomedEText;
        public TMPro.TMP_Text zoomedSText;
        public TMPro.TMP_Text zoomedWText;
        public TMPro.TMP_Text zoomedCText;
        public TMPro.TMP_Text zoomedAText;
        public TMPro.TMP_Text unzoomedNText;
        public TMPro.TMP_Text unzoomedEText;
        public TMPro.TMP_Text unzoomedSText;
        public TMPro.TMP_Text unzoomedWText;
        public TMPro.TMP_Text unzoomedCText;
        public TMPro.TMP_Text unzoomedAText;

        public int N 
        {
            set
            {
                var str = $"N\n{value}";
                zoomedNText.text = str;
                unzoomedNText.text = str;
            }
        }
        public int E
        {
            set
            {
                var str = $"E\n{value}";
                zoomedEText.text = str;
                unzoomedEText.text = str;
            }
        }
        public int S
        {
            set
            {
                var str = $"S\n{value}";
                zoomedSText.text = str;
                unzoomedSText.text = str;
            }
        }
        public int W
        {
            set
            {
                var str = $"W\n{value}";
                zoomedWText.text = str;
                unzoomedWText.text = str;
            }
        }
        public int C
        {
            set
            {
                var str = $"C\n{value}";
                zoomedCText.text = str;
                unzoomedCText.text = str;
            }
        }
        public int A
        {
            set
            {
                var str = $"A\n{value}";
                zoomedAText.text = str;
                unzoomedAText.text = str;
            }
        }

        public virtual void SetPhysicalLocation(CardLocation location)
        {
            Debug.Log($"Card controller of {card.CardName} setting physical location in {card.Location} to {card.BoardX}, {card.BoardY}");
            switch (location)
            {
                case CardLocation.Deck:
                    card.gameObject.transform.SetParent(card.Controller.deckObject.transform);
                    gameObject.SetActive(false);
                    break;
                case CardLocation.Discard:
                    card.gameObject.transform.SetParent(card.Controller.discardObject.transform);
                    card.Controller.discardCtrl.SpreadOutCards();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Field:
                    card.gameObject.transform.localScale = Vector3.one;
                    card.gameObject.transform.SetParent(card.Game.boardObject.transform);
                    MoveTo((card.BoardX, card.BoardY));
                    SetRotation();
                    //Card game object active-ness is set in moveTo
                    break;
                case CardLocation.Hand:
                    card.gameObject.transform.SetParent(card.Controller.handObject.transform);
                    card.Controller.handCtrl.SpreadOutCards();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Annihilation:
                    gameObject.SetActive(false);
                    break;
                default: throw new System.ArgumentException($"Invalid card location {location} to put card physically at");
            }
        }

        public void SetRotation()
        {
            card.gameObject.transform.eulerAngles = new Vector3(0, 180 + 180 * card.ControllerIndex, 0);
        }

        /// <summary>
        /// Set the sprites of this card and gameobject
        /// </summary>
        public void SetImage(string cardFileName, bool zoomed)
        {
            Texture pic;
            if (zoomed) pic = Resources.Load<Texture>("Card Detailed Textures/" + cardFileName);
            else pic = Resources.Load<Texture>("Unzoomed Card Textures/" + cardFileName);

            //check if either is null. if so, log to debug and return
            if (pic == null)
            {
                Debug.Log("Could not find sprite with name " + cardFileName);
                return;
            }

            cardFaceRenderer.material.mainTexture = pic;
        }

        public void ShowForCardType(char cardType, bool zoomed)
        {
            bool isChar = cardType == 'C';

            bool zoomedChar = isChar && zoomed;
            zoomedNText.gameObject.SetActive(zoomedChar);
            zoomedEText.gameObject.SetActive(zoomedChar);
            zoomedSText.gameObject.SetActive(zoomedChar);
            zoomedWText.gameObject.SetActive(zoomedChar);
            zoomedCharFrame.SetActive(zoomedChar);

            bool unzoomedChar = isChar && !zoomed;
            unzoomedNText.gameObject.SetActive(unzoomedChar);
            unzoomedEText.gameObject.SetActive(unzoomedChar);
            unzoomedSText.gameObject.SetActive(unzoomedChar);
            unzoomedWText.gameObject.SetActive(unzoomedChar);
            unzoomedCharFrame.SetActive(unzoomedChar);

            zoomedCText.gameObject.SetActive(cardType == 'S' && zoomed);
            unzoomedCText.gameObject.SetActive(cardType == 'S' && !zoomed);

            zoomedAText.gameObject.SetActive(cardType == 'A' && zoomed);
            unzoomedAText.gameObject.SetActive(cardType == 'A' && !zoomed);

            zoomedAllFrame.SetActive(zoomed);
            unzoomedAllFrame.SetActive(!zoomed);

            SetImage(card.CardName, zoomed);
        }

        /// <summary>
        /// Sets this card's x and y values and updates its transform
        /// </summary>
        private void MoveTo((int x, int y) to)
        {
            transform.localPosition = BoardController.GridIndicesToPosWithStacking(to.x, to.y, card.Augments.Count);
            gameObject.SetActive(card.AugmentedCard == null);
        }
    }
}