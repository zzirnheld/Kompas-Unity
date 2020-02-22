using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace KompasNetworking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public Player Player;

        public override void Update()
        {
            //first check to see if there's any data to read
            base.Update();

            if (Packets.Count == 0) return;

            //process packet
            //TODO
        }
    }
}
