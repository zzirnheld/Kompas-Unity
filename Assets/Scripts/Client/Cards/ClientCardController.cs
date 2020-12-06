using KompasClient.Cards;
using KompasCore.Cards;

public class ClientCardController : CardController
{
    public override void SetPhysicalLocation(CardLocation location)
    {
        base.SetPhysicalLocation(location);

        ShowForCardType(card.CardType, ClientCameraController.Main.Zoomed);
    }
}
