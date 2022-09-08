using KompasClient.GameCore;
using KompasClient.UI;
using KompasCore.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.Cards
{
    public class ClientCardController : CardController
    {
        public ClientGameCard clientCard;

        [Header("Card display objects")]
        public Image nUnzoomedImage;
        public Image eUnzoomedImage;
        public Image sUnzoomedImage;
        public Image wUnzoomedImage;
        public Image cUnzoomedImage;
        public Image aUnzoomedImage;

        public Image zoomedHazeImage;
        public Sprite zoomedCharHaze;
        public Sprite zoomedNonCharHaze;
        public Image unzoomedHazeImage;
        public Sprite unzoomedCharHaze;
        public Sprite unzoomedNonCharHaze;

        public GameObject revealedImage;

        public Material friendlyCardFrameMaterial;
        public Material enemyCardFrameMaterial;

        public Renderer[] frameObjects;

        [Header("Dependent MonoBehaviours")]
        public ClientCardMouseController mouseController;

        public override GameCard Card => clientCard;

        public ClientGame ClientGame => clientCard.ClientGame;
        public ClientUIController ClientUIController => ClientGame.clientUICtrl;

        public bool Revealed
        {
            set => revealedImage.SetActive(value);
        }

        private void OnDestroy()
        {
            Debug.Log("Destroying a client card ctrl. Destroying this ctrl's mouse ctrl.");
            Destroy(mouseController);
        }

        public override void SetPhysicalLocation(CardLocation location)
        {
            base.SetPhysicalLocation(location);

            ShowForCardType(Card.CardType, ClientCameraController.Main.Zoomed);
        }

        public override void SetImage(string cardFileName, bool zoomed)
        {
            base.SetImage(cardFileName, zoomed);

            zoomedHazeImage.enabled = zoomed;
            zoomedHazeImage.sprite = Card.CardType == 'C' ? zoomedCharHaze : zoomedNonCharHaze;

            unzoomedHazeImage.enabled = !zoomed;
            unzoomedHazeImage.sprite = Card.CardType == 'C' ? unzoomedCharHaze : unzoomedNonCharHaze;
        }

        //TODO move this out to something inheriting from CardViewController?
        public void ApplySettings(ClientSettings settings)
        {
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

            Debug.Log($"setting color to {settings.FriendlyColor}");
            friendlyCardFrameMaterial.color = settings.FriendlyColor;
            enemyCardFrameMaterial.color = settings.EnemyColor;
            ShowFrameColor();
        }

        private void ShowFrameColor()
        {
            if (Card.Controller == null)
            {
                //no controller yet, don't bother showing color
                return;
            }
            Material material = Card.Controller.Friendly ? friendlyCardFrameMaterial : enemyCardFrameMaterial;
            foreach (var obj in frameObjects)
            {
                obj.material = material;
            }
        }

        public override void ShowForCardType(char cardType, bool zoomed)
        {
            base.ShowForCardType(cardType, zoomed);
            ShowFrameColor();
        }
    }
}