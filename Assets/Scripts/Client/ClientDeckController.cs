using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDeckController : DeckController
{
    public ClientGame ClientGame;

    public List<ClientGameCard> ClientDeck { get; } = new List<ClientGameCard>();
    public override List<GameCard> Deck => ClientDeck;

    public void OnMouseDown()
    {
        //request a draw
        if (ClientGame.friendlyDeckCtrl == this)
            ClientGame.clientNotifier.RequestDraw();
    }
}
