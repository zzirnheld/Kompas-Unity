using KompasClient.Cards;
using KompasCore.GameCore;
using System.Collections.Generic;

namespace KompasClient.GameCore
{
    public class ClientDeckController : DeckController
    {
        public ClientGame ClientGame;

        public List<ClientGameCard> ClientDeck { get; } = new List<ClientGameCard>();

        public void OnMouseDown()
        {
            //request a draw
            if (ClientGame.friendlyDeckCtrl == this)
                ClientGame.clientNotifier.RequestDraw();
        }
    }
}
