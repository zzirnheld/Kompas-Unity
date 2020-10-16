using KompasCore.GameCore;
using UnityEngine;
using TMPro;
using KompasCore.UI;

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

        public MeshRenderer cardFaceRenderer;
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

        public GameObject currentTargetObject;
        public GameObject validTargetObject;

        public OscillatingController attackOscillator;
        public OscillatingController effectOscillator;

        public int N 
        {
            set
            {
                zoomedNText.text = $"N\n{value}";
                unzoomedNText.text = $"{value}";
                unzoomedNText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }
        public int E
        {
            set
            {
                zoomedEText.text = $"E\n{value}";
                unzoomedEText.text = $"{value}";
                unzoomedEText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }
        public int S
        {
            set
            {
                zoomedSText.text = $"S\n{value}";
                unzoomedSText.text = $"{value}";
                unzoomedSText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }
        public int W
        {
            set
            {
                zoomedWText.text = $"W\n{value}";
                unzoomedWText.text = $"{value}";
                unzoomedWText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }
        public int C
        {
            set
            {
                zoomedCText.text = $"C\n{value}";
                unzoomedCText.text = $"{value}";
                unzoomedCText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }
        public int A
        {
            set
            {
                zoomedAText.text = $"A\n{value}";
                unzoomedAText.text = $"{value}";
                unzoomedAText.fontSize = value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;
            }
        }

        public virtual void SetPhysicalLocation(CardLocation location)
        {
            Debug.Log($"Card controller of {card.CardName} setting physical location in {card.Location} to {card.BoardX}, {card.BoardY}");

            aoeController.Hide();

            if(card.AugmentedCard != null)
            {
                gameObject.SetActive(true);
                card.AugmentedCard.cardCtrl.SpreadOutAugs();
                return;
            }

            card.transform.localScale = Vector3.one;

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

            SpreadOutAugs();
        }

        public void SpreadOutAugs()
        {
            var augCount = card.Augments.Count;
            /*float scale = 0.4f / ((float) (augCount / 4));
            float increment = 0.4f/ ((float) (augCount / 4)); //1f / ((float) (2f * augCount))
            float xOffset = -1f + increment;
            float zOffset = 1f - increment;
            foreach(var aug in card.Augments)
            {
                aug.transform.parent = card.transform;
                aug.transform.localScale = new Vector3(scale, scale, scale);
                aug.transform.localPosition = new Vector3(xOffset, 0.2f, zOffset);
                xOffset += increment + increment;
            }*/
            float scale = 0.4f / ((float)((augCount + 3) / 4));
            int i = 0;
            foreach(var aug in card.Augments)
            {
                aug.transform.parent = card.transform;
                aug.transform.localScale = new Vector3(scale, scale, scale);
                float x, z;
                switch(i % 4)
                {
                    case 0: (x, z) = (-0.5f, 0.5f); break;
                    case 1: (x, z) = (0.5f, 0.5f); break;
                    case 2: (x, z) = (0.5f, -0.5f); break;
                    case 3: (x, z) = (-0.5f, -0.5f); break;
                    default: (x, z) = (0f, 0f); break;
                }
                aug.transform.localPosition = new Vector3(x, 0.2f * ((i / 4) + 1), z);
                i++;
            }
        }

        public void SetRotation()
            => card.transform.eulerAngles = new Vector3(0, 180 + 180 * card.ControllerIndex, 0);

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

            zoomedAllFrame.SetActive(zoomed);
            unzoomedAllFrame.SetActive(!zoomed);

            //the following logic is arranged the way it is so you don't loop through all cards,
            //unless the card does actually have a possible attack

            //only check if card can attack if on field
            if (card.Location == CardLocation.Field)
            {
                //if you can attack at all, enable the attack indicator
                if (card.AttackRestriction.EvaluateAtAll())
                    //oscillate the attack indicator if can attack a card right now
                    attackOscillator.Enable(card.AttackRestriction.EvaluateAny());
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

        /// <summary>
        /// Sets this card's x and y values and updates its transform
        /// </summary>
        private void MoveTo((int x, int y) to)
        {
            int heightIndex = card.AugmentedCard == null ? card.Augments.Count : card.AugmentedCard.Augments.IndexOf(card);
            transform.localPosition = BoardController.GridIndicesToPosWithStacking(to.x, to.y, heightIndex);
            gameObject.SetActive(card.AugmentedCard == null);

            if (card.CardType == 'S' && card.SpellSubtype == CardBase.RadialSubtype) aoeController.Show(card.Arg);
        }

        public void ShowValidTarget(bool valid = true) => validTargetObject.SetActive(valid);

        public void ShowCurrentTarget(bool current = true) => currentTargetObject.SetActive(current);

        public void HideTarget()
        {
            validTargetObject.SetActive(false);
            currentTargetObject.SetActive(false);
        }
    }
}