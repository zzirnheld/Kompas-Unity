using KompasClient.Cards;
using KompasClient.UI;
using KompasCore.Cards;
using UnityEngine;
using UnityEngine.UI;

public class ClientCardController : CardController
{
    public Image nUnzoomedImage;
    public Image eUnzoomedImage;
    public Image sUnzoomedImage;
    public Image wUnzoomedImage;
    public Image cUnzoomedImage;
    public Image aUnzoomedImage;

    public Image hazeImage;
    public Sprite charHaze;
    public Sprite nonCharHaze;

    public GameObject revealedImage;

    public ClientCardMouseController mouseCtrl;

    public bool Revealed
    {
        set => revealedImage.SetActive(value);
    }

    private void OnDestroy()
    {
        Destroy(mouseCtrl);
    }

    public override void SetPhysicalLocation(CardLocation location)
    {
        base.SetPhysicalLocation(location);

        ShowForCardType(card.CardType, ClientCameraController.Main.Zoomed);
    }

    public override void SetImage(string cardFileName, bool zoomed)
    {
        base.SetImage(cardFileName, zoomed);

        hazeImage.enabled = zoomed;
        hazeImage.sprite = card.CardType == 'C' ? charHaze : nonCharHaze;
    }

    public void ApplySettings(ClientUISettings settings)
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
    }
}
