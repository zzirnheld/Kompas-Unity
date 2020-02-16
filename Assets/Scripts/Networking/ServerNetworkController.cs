using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace KompasNetworking
{
    //handles networking and such for a server game
    public class ServerNetworkController : NetworkController
    {
        public bool Hosting { get; private set; }

        private TcpClient[] clients;
        private int numClients = 0;

        public bool Connect(TcpClient tcpClient)
        {
            if (numClients >= 2)
            {
                throw new System.IndexOutOfRangeException("Too many clients tried to connect to a single server network controller!");
            }

            clients[numClients] = tcpClient;
            numClients++;

            if(numClients >= 2)
            {

                return true;
            }
            else
            {
                return false;
            }
        }

        private void StartGame()
        {
            //TODO
        }

        void Update()
        {
            //TODO
        }
    }
}
