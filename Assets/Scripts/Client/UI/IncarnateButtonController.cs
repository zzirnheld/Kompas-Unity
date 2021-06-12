using KompasClient.GameCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KompasClient.UI
{
    public class IncarnateButtonController : MonoBehaviour
    {
        public ClientPlayer clientPlayer;

        public void OnMouseUp()
        {
            clientPlayer.Incarnate();
        }
    }
}