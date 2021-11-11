using KompasCore.GameCore;
using UnityEngine;
using TMPro;
using KompasCore.UI;
using UnityEngine.UI;
using System.Linq;

namespace KompasCore.Cards
{
    /// <summary>
    /// Controls the card's physical behavior.
    /// </summary>
    public class CardController : MonoBehaviour
    {
        public const float LargeUnzoomedTextFontSize = 32f;
        public const float SmallUnzoomedTextFontSize = 22f;

        public GameCard card;

        public CardAOEController aoeController;

        //public MeshRenderer cardFaceRenderer;
        public GameObject zoomedCharFrame;
        public GameObject zoomedAllFrame;
        public GameObject unzoomedCharFrame;
        public GameObject unzoomedAllFrame;

        public GameObject zoomedCharStatBackgrounds;
        public GameObject unzoomedCharStatBackgrounds;
        public GameObject zoomedSpellStatBackgrounds;
        public GameObject unzoomedSpellStatBackgrounds;
        public GameObject zoomedAugStatBackgrounds;
        public GameObject unzoomedAugStatBackgrounds;

        public TMP_Text zoomedNText;
        public TMP_Text zoomedEText;
        public TMP_Text zoomedSText;
        public TMP_Text zoomedWText;
        public TMP_Text zoomedCText;
        public TMP_Text zoomedAText;
        public TMP_Text unzoomedNText;
        public TMP_Text unzoomedEText;
        public TMP_Text unzoomedSText;
        public TMP_Text unzoomedWText;
        public TMP_Text unzoomedCText;
        public TMP_Text unzoomedAText;

        public TMP_Text zoomedNameText;
        public TMP_Text zoomedSubtypesText;
        public TMP_Text zoomedEffText;

        public GameObject currentTargetObject;
        public GameObject validTargetObject;
        public GameObject uniqueCopyObject;
        public GameObject primaryStackableObject;
        public GameObject secondaryStackableObject;

        public OscillatingController attackOscillator;
        public OscillatingController effectOscillator;

        private string currImageCardName;
        private bool currImageZoomLevel;
        private Sprite cardImageSprite;
        public Image cardImageImage;
        //public Image zoomMaskImage;

        public int N 
        {
            set
            {
                zoomedNText.text = $"N\n{value}";
                unzoomedNText.text = $"{value}";
                unzoomedNText.fontSize = FontSizeForValue(value);
            }
        }
        public int E
        {
            set
            {
                zoomedEText.text = $"E\n{value}";
                unzoomedEText.text = $"{value}";
                unzoomedEText.fontSize = FontSizeForValue(value);
            }
        }
        public int S
        {
            set
            {
                zoomedSText.text = $"S\n{value}";
                unzoomedSText.text = $"{value}";
                unzoomedSText.fontSize = FontSizeForValue(value);
            }
        }
        public int W
        {
            set
            {
                zoomedWText.text = $"W\n{value}";
                unzoomedWText.text = $"{value}";
                unzoomedWText.fontSize = FontSizeForValue(value);
            }
        }
        public int C
        {
            set
            {
                zoomedCText.text = $"C\n{value}";
                unzoomedCText.text = $"{value}";
                unzoomedCText.fontSize = FontSizeForValue(value);
            }
        }
        public int A
        {
            set
            {
                zoomedAText.text = $"A\n{value}";
                unzoomedAText.text = $"{value}";
                unzoomedAText.fontSize = FontSizeForValue(value);
            }
        }

        private float FontSizeForValue(int value) => value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;

        public virtual void SetPhysicalLocation(CardLocation location)
        {
            //Debug.Log($"Card controller of {card.CardName} setting physical location in {card.Location} to {card.BoardX}, {card.BoardY}");

            aoeController.Hide();

            //is the card augmenting something?
            if(card.AugmentedCard != null)
            {
                gameObject.SetActive(true);
                card.AugmentedCard.cardCtrl.SpreadOutAugs();
                return;
            }

            //Here on out, we assume the card's not an augment
            card.transform.localScale = Vector3.one;

            switch (location)
            {
                case CardLocation.Nowhere: break;
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
                    MoveTo(card.Position);
                    SetRotation();
                    if (card.CardType == 'S' && card.SpellSubtypes.Any(s => s == CardBase.RadialSubtype)) aoeController.Show(card.Radius);
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Hand:
                    card.gameObject.transform.SetParent(card.Controller.handObject.transform);
                    card.Controller.handCtrl.SpreadOutCards();
                    gameObject.SetActive(true);
                    break;
                case CardLocation.Annihilation:
                    card.gameObject.transform.SetParent(card.Controller.annihilationCtrl.gameObject.transform);
                    card.Controller.annihilationCtrl.SpreadOutCards();
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
            transform.localPosition = BoardController.GridIndicesToCardPos(to.x, to.y);
        }

        public void SpreadOutAugs()
        {
            var augCount = card.AugmentsList.Count;
            float scale = 0.4f; // / ((float)((augCount + 3) / 4));
            int i = 0;
            foreach(var aug in card.AugmentsList)
            {
                aug.transform.parent = card.transform;
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
            }
        }

        public void SetRotation()
            => card.transform.eulerAngles = new Vector3(0, 180 + 180 * card.ControllerIndex, 0);

        private void ReloadImages(string cardFileName)
        {
            cardImageSprite = Resources.Load<Sprite>("Simple Sprites/" + cardFileName);
        }

        /// <summary>
        /// Set the sprites of this card and gameobject
        /// </summary>
        public virtual void SetImage(string cardFileName, bool zoomed)
        {
            if (cardFileName == currImageCardName && currImageZoomLevel == zoomed) return;
            if (currImageCardName != cardFileName) ReloadImages(cardFileName);

            currImageCardName = cardFileName;
            currImageZoomLevel = zoomed;

            //cardFaceRenderer.material.mainTexture = zoomed ? zoomedInTex : zoomedOutTex;
            cardImageImage.sprite = cardImageSprite;
            //zoomMaskImage.enabled = zoomed;
        }


        /// <summary>
        /// Sets frame and text objects based on the card's type, and zoom level
        /// </summary>
        /// <param name="cardType">Whether the card is a character, spell, or augment.</param>
        /// <param name="zoomed">Whether to show the card as zoomed in or not.</param>
        public void ShowForCardType(char cardType, bool zoomed)
        {
            bool isChar = cardType == 'C';

            bool zoomedChar = isChar && zoomed;
            zoomedNText.gameObject.SetActive(zoomedChar);
            zoomedEText.gameObject.SetActive(zoomedChar);
            zoomedSText.gameObject.SetActive(zoomedChar);
            zoomedWText.gameObject.SetActive(zoomedChar);
            zoomedCharFrame.SetActive(zoomedChar);
            zoomedCharStatBackgrounds.SetActive(zoomedChar);

            bool unzoomedChar = isChar && !zoomed;
            unzoomedNText.gameObject.SetActive(unzoomedChar);
            unzoomedEText.gameObject.SetActive(unzoomedChar);
            unzoomedSText.gameObject.SetActive(unzoomedChar);
            unzoomedWText.gameObject.SetActive(unzoomedChar);
            unzoomedCharFrame.SetActive(unzoomedChar);
            unzoomedCharStatBackgrounds.SetActive(unzoomedChar);

            bool zoomedSpell = cardType == 'S' && zoomed;
            zoomedCText.gameObject.SetActive(zoomedSpell);
            zoomedSpellStatBackgrounds.SetActive(zoomedSpell);
            bool unzoomedSpell = cardType == 'S' && !zoomed;
            unzoomedCText.gameObject.SetActive(unzoomedSpell);
            unzoomedSpellStatBackgrounds.SetActive(unzoomedSpell);

            bool zoomedAug = cardType == 'A' && zoomed;
            zoomedAText.gameObject.SetActive(zoomedAug);
            zoomedAugStatBackgrounds.SetActive(zoomedAug);
            bool unzoomedAug = cardType == 'A' && !zoomed;
            unzoomedAText.gameObject.SetActive(unzoomedAug);
            unzoomedAugStatBackgrounds.gameObject.SetActive(unzoomedAug);

            zoomedNameText.gameObject.SetActive(zoomed);
            zoomedSubtypesText.gameObject.SetActive(zoomed);
            zoomedEffText.gameObject.SetActive(zoomed);
            zoomedNameText.text = card.CardName;
            zoomedSubtypesText.text = card.QualifiedSubtypeText;
            zoomedEffText.text = card.EffText;

            zoomedAllFrame.SetActive(zoomed);
            unzoomedAllFrame.SetActive(!zoomed);

            //the following logic is arranged the way it is so you don't loop through all cards,
            //unless the card does actually have a possible attack

            //only check if card can attack if on field
            if (card.Location == CardLocation.Field)
            {
                //if you can attack at all, enable the attack indicator
                if (card.AttackRestriction.EvaluateAtAll(null))
                    //oscillate the attack indicator if can attack a card right now
                    attackOscillator.Enable(card.AttackRestriction.EvaluateAny(null));
                else attackOscillator.Disable();

                //if you can activate any effect, enable the attack indicator
                if (card.HasAtAllActivateableEffect)
                    //oscillate the effect indicator if you can activate an effect right now
                    effectOscillator.Enable(card.HasCurrentlyActivateableEffect);
                else effectOscillator.Disable();
            }
            else
            {
                attackOscillator.Disable();
                effectOscillator.Disable();
            }

            SetImage(card.CardName, zoomed);
        }

        public void ShowValidTarget(bool valid = true) => validTargetObject.SetActive(valid);

        public void ShowCurrentTarget(bool current = true) => currentTargetObject.SetActive(current);

        public void HideTarget()
        {
            validTargetObject.SetActive(false);
            currentTargetObject.SetActive(false);
        }

        public void ShowUniqueCopy(bool copy = true) => uniqueCopyObject.SetActive(copy);

        public void ShowPrimaryOfStackable(bool show = true) => primaryStackableObject.SetActive(show);
        public void ShowSecondaryOfStackable(bool show = true) => secondaryStackableObject.SetActive(show);
    }
}