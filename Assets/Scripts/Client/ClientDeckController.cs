using KompasClient.UI;
using KompasCore.GameCore;
using TMPro;

namespace KompasClient.GameCore
{
    public class ClientDeckController : DeckController
    {
        public ClientPlayer owner;

        public override Player Owner => owner;

        public ClientDeckUIController deckUIController;

        public TMP_Text deckCountLabel;
        public int DeckCount
        {
            set
            {
                deckCountLabel.text = $"{value}";
            }
        }

        public void OnMouseDown()
        {
            //request a draw
            if (Game.Players[0].deckCtrl == this) owner.game.clientNotifier.RequestDraw();
        }

        public override void Refresh() => deckUIController.Refresh();
    }
}
