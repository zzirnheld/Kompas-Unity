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

        private bool Zoomed => overrideZoom ? zoomOverrideValue : ClientCameraController.MainZoomed;

        private bool overrideZoom;
        private bool zoomOverrideValue;

        public CardAOEController aoeController;

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

        public VoxelCardUser voxelCardUser;

        public GameObject zoomedUI;
        public GameObject unzoomedUI;
        public GameObject[] charUIs;
        //public GameObject[] nonCharUIs;

        /// <summary>
        /// Used to make sure we don't regenerate the texture unnecessarily
        /// </summary>
        private string oldFileName;

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
            handleStatColors(nText, eText, costText, wText);
            handleStatColors(unzoomedNText, unzoomedEText, unzoomedCostText, unzoomedWText);

            bool isChar = ShownCard.CardType == 'C';
            voxelCardUser.Set(isChar, Zoomed, default);

            unzoomedUI.SetActive(!Zoomed);
            zoomedUI.SetActive(Zoomed);
            foreach (var go in charUIs) go.SetActive(isChar);
            //foreach (var go in nonCharUIs) go.SetActive(!isChar);
        }

        private static bool HasCurrentlyActivateableEffect(GameCard card)
            => card.Effects != null && card.Effects.Count(e => e.CanBeActivatedBy(card.Controller)) > 0;

        private static bool HasAtAllActivateableEffect(GameCard card)
            => card.Effects != null && card.Effects.Count(e => e.CanBeActivatedAtAllBy(card.Controller)) > 0;

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
                if (HasAtAllActivateableEffect(ShownGameCard))
                    //oscillate the effect indicator if you can activate an effect right now
                    effectOscillator.Enable(HasCurrentlyActivateableEffect(ShownGameCard));
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
                /*
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
                enemyCardFrameMaterial.color = settings.EnemyColor;*/
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

        protected override string DisplayN(int n) => $"{n}";
        protected override string DisplayE(int e) => $"{e}";
        protected override string DisplayS(int s) => $"{s}";
        protected override string DisplayW(int w) => $"{w}";
        protected override string DisplayC(int c) => $"{c}";
        protected override string DisplayA(int a) => $"{a}";

        protected override void DisplayCardImage(Sprite cardImageSprite)
        {
            if (oldFileName == ShownCard.FileName) return;

            base.DisplayCardImage(cardImageSprite);
            //TODO split this out if I ever make chars able to become spells or vice versa
            voxelCardUser.Set(ShownCard.CardType == 'C', Zoomed, cardImageSprite);

            oldFileName = ShownCard.FileName;
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
            /*
            Material material = ShownGameCard.Controller.Friendly ? friendlyCardFrameMaterial : enemyCardFrameMaterial;
            if (material == null) return; //Could be an error (TODO handle) but could be just a server card

            foreach (var obj in frameObjects)
            {
                obj.material = material;
            }*/
        }
    }
}