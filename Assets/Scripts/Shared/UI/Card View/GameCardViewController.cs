using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace KompasCore.UI
{
    [RequireComponent(typeof(CardController))]
    public class GameCardViewController : GameCardlikeViewController
    {
        public const float LargeUnzoomedTextFontSize = 32f;
        public const float SmallUnzoomedTextFontSize = 22f;
        private static float UnzoomedFontSizeForValue(int value) => value < 10 ? LargeUnzoomedTextFontSize : SmallUnzoomedTextFontSize;

        private static bool Zoomed => ClientCameraController.MainZoomed;

        public CardAOEController aoeController;

        //public MeshRenderer cardFaceRenderer;
        [Header("Card frame")]
        public GameObject zoomedCharFrame;
        public GameObject zoomedAllFrame;
        public GameObject unzoomedCharFrame;
        public GameObject unzoomedAllFrame;

        [Header("Stat backgrounds (the colored things)")]
        public GameObject zoomedCharStatBackgrounds;
        public GameObject zoomedSpellStatBackgrounds;
        public GameObject zoomedAugStatBackgrounds;

        public GameObject unzoomedCharStatBackgrounds;
        public GameObject unzoomedSpellStatBackgrounds;
        public GameObject unzoomedAugStatBackgrounds;

        public Image nUnzoomedImage;
        public Image eUnzoomedImage;
        public Image sUnzoomedImage;
        public Image wUnzoomedImage;
        public Image cUnzoomedImage;
        public Image aUnzoomedImage;

        //Zoomed-in text is handled by base class
        [Header("Zoomed-out card numeric stats")]
        public TMP_Text unzoomedNText;
        public TMP_Text unzoomedEText;
        public TMP_Text unzoomedCostText;
        public TMP_Text unzoomedWText;

        //TODO give these to dummy cards, as empties probably
        [Header("Card highlighting")]
        public GameObject uniqueCopyObject;
        public GameObject linkedCardObject;
        public GameObject primaryStackableObject;
        public GameObject secondaryStackableObject;

        [Header("Can attack/effect indicators")]
        public OscillatingController attackOscillator;
        public OscillatingController effectOscillator;

        [Header("Zoom level haze")]
        public Image zoomedHazeImage;
        public Sprite zoomedCharHaze;
        public Sprite zoomedNonCharHaze;

        public Image unzoomedHazeImage;
        public Sprite unzoomedCharHaze;
        public Sprite unzoomedNonCharHaze;

        [Header("Frame coloring")]
        public Material friendlyCardFrameMaterial;
        public Material enemyCardFrameMaterial;

        public Renderer[] frameObjects;

        protected override void Display()
        {
            base.Display();

            HandleZoom();
        }

        protected override bool ShowingInfo
        {
            set => base.ShowingInfo = value && ShownGameCard?.Location != CardLocation.Deck && ShownGameCard?.Location != CardLocation.Nowhere;
        }

        protected virtual void HandleZoom()
        {
            bool isChar = shownCard.CardType == 'C';

            //Zoomed in things
            nText.gameObject.SetActive(Zoomed && isChar);
            eText.gameObject.SetActive(Zoomed && isChar);
            wText.gameObject.SetActive(Zoomed && isChar);

            zoomedCharFrame.SetActive(Zoomed && isChar);

            costText.gameObject.SetActive(Zoomed);
            nameText.gameObject.SetActive(Zoomed);
            subtypesText.gameObject.SetActive(Zoomed);
            effText.gameObject.SetActive(Zoomed);

            zoomedAllFrame.SetActive(Zoomed);

            //Unzoomed things
            unzoomedNText.gameObject.SetActive(!Zoomed && isChar);
            unzoomedEText.gameObject.SetActive(!Zoomed && isChar);
            unzoomedWText.gameObject.SetActive(!Zoomed && isChar);

            unzoomedCharFrame.SetActive(!Zoomed && isChar);

            unzoomedCostText.gameObject.SetActive(!Zoomed);

            unzoomedAllFrame.SetActive(!Zoomed);

            //Handle font sizes
            unzoomedNText.fontSize = UnzoomedFontSizeForValue(shownCard.N);
            unzoomedEText.fontSize = UnzoomedFontSizeForValue(shownCard.E);
            unzoomedWText.fontSize = UnzoomedFontSizeForValue(shownCard.W);

            unzoomedCostText.fontSize = UnzoomedFontSizeForValue(shownCard.Cost);
        }

        protected override void DisplaySpecialEffects()
        {
            base.DisplaySpecialEffects();
            ShowFrameColor();

            if (ShownGameCard.Location == CardLocation.Board)
            {
                //if you can attack at all, enable the attack indicator
                if (ShownGameCard.AttackRestriction.CouldAttackValidTarget(stackSrc: null))
                    //oscillate the attack indicator if can attack a card right now
                    attackOscillator.Enable(ShownGameCard.AttackRestriction.CanAttackAnyCard(stackSrc: null));
                else attackOscillator.Disable();

                //if you can activate any effect, enable the attack indicator
                if (ShownGameCard.HasAtAllActivateableEffect)
                    //oscillate the effect indicator if you can activate an effect right now
                    effectOscillator.Enable(ShownGameCard.HasCurrentlyActivateableEffect);
                else effectOscillator.Disable();

                if (shownCard.SpellSubtypes.Any(CardBase.RadialSubtype.Equals)) aoeController.Show(shownCard.Radius);
            }
            else
            {
                attackOscillator.Disable();
                effectOscillator.Disable();
                aoeController.Hide();
            }

            //TODO move settings off of client and into shared
            if (ShownGameCard.Game is ClientGame clientGame)
            {
                var settings = clientGame.clientUIController.clientUISettingsController.ClientSettings;
                switch (settings.statHighlight)
                {
                    case StatHighlight.NoHighlight:
                        nUnzoomedImage.gameObject.SetActive(false);
                        eUnzoomedImage.gameObject.SetActive(false);
                        sUnzoomedImage.gameObject.SetActive(false);
                        wUnzoomedImage.gameObject.SetActive(false);
                        cUnzoomedImage.gameObject.SetActive(false);
                        aUnzoomedImage.gameObject.SetActive(false);
                        break;
                    case StatHighlight.ColoredBack:
                        nUnzoomedImage.gameObject.SetActive(true);
                        eUnzoomedImage.gameObject.SetActive(true);
                        sUnzoomedImage.gameObject.SetActive(true);
                        wUnzoomedImage.gameObject.SetActive(true);
                        cUnzoomedImage.gameObject.SetActive(true);
                        aUnzoomedImage.gameObject.SetActive(true);
                        break;
                    default: throw new System.ArgumentException($"Invalid stat highlight setting {settings.statHighlight}");
                }

                //Debug.Log($"setting color to {settings.FriendlyColor}");
                friendlyCardFrameMaterial.color = settings.FriendlyColor;
                enemyCardFrameMaterial.color = settings.EnemyColor;
            }
        }

        protected override void DisplayCardNumericStats()
        {
            base.DisplayCardNumericStats();

            unzoomedNText.text = $"{shownCard.N}";
            unzoomedEText.text = $"{shownCard.E}";
            unzoomedWText.text = $"{shownCard.W}";

            unzoomedCostText.text = $"{shownCard.Cost}";
        }

        protected override void DisplayCardImage()
        {
            base.DisplayCardImage();

            zoomedHazeImage.enabled = Zoomed;
            zoomedHazeImage.sprite = shownCard.CardType == 'C' ? zoomedCharHaze : zoomedNonCharHaze;

            unzoomedHazeImage.enabled = !Zoomed;
            unzoomedHazeImage.sprite = shownCard.CardType == 'C' ? unzoomedCharHaze : unzoomedNonCharHaze;
        }

        public virtual void ShowUniqueCopy(bool copy = true) => uniqueCopyObject.SetActive(copy);

        public virtual void ShowLinkedCard(bool show = true) => linkedCardObject.SetActive(show);

        public virtual void ShowPrimaryOfStackable(bool show = true) => primaryStackableObject.SetActive(show);
        public virtual void ShowSecondaryOfStackable(bool show = true) => secondaryStackableObject.SetActive(show);

        private void ShowFrameColor()
        {
            if (ShownGameCard.Controller == null)
            {
                //no controller yet, don't bother showing color
                return;
            }
            Material material = ShownGameCard.Controller.Friendly ? friendlyCardFrameMaterial : enemyCardFrameMaterial;
            if (material == null) return; //Could be an error (TODO handle) but could be just a server card

            foreach (var obj in frameObjects)
            {
                obj.material = material;
            }
        }
    }
}