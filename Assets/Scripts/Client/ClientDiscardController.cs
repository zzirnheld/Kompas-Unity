using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientDiscardController : DiscardController
{
    public ClientGame ClientGame;

    public void OnMouseDown()
    {
        if (ClientGame.friendlyDiscardCtrl == this)
            ClientGame.clientNotifier.RequestRehand(GetLastDiscarded());
    }
}
