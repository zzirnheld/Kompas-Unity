using KompasCore.GameCore;
using TMPro;

namespace KompasClient.GameCore
{
    public class ClientDeckController : DeckController
    {
        public ClientGame ClientGame;

        public ClientPlayer owner;
        public override Player Owner => owner;

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
            if (ClientGame.Players[0].deckCtrl == this) ClientGame.clientNotifier.RequestDraw();
        }

        /*private void UpdateDeckCount() => DeckCount = Deck.Count;

        protected override bool AddCard(GameCard card, IStackable stackSrc = null)
        {
            var success = base.AddCard(card, stackSrc);
            UpdateDeckCount();
            return success;
        }

        public override bool RemoveFromDeck(GameCard card)
        {
            var success = base.RemoveFromDeck(card);
            UpdateDeckCount();
            return success;
        }*/
    }
}
