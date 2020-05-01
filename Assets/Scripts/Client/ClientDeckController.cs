using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDeckController : DeckController
{
    public ClientGame ClientGame;

    public void OnMouseDown()
    {
        //request a draw
        if (ClientGame.friendlyDeckCtrl == this)
            ClientGame.clientNotifier.RequestDraw();
    }
}
