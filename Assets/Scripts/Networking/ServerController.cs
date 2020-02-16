using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace KompasNetworking
{
    public class ServerController : MonoBehaviour
    {
        public GameObject GamePrefab;

        private TcpListener listener;
        private List<ServerNetworkController> games;
        private ServerNetworkController currGame = null;

        private void Awake()
        {
            IPAddress ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

            games = new List<ServerNetworkController>();
            try
            {
                listener = new TcpListener(ipAddress, NetworkController.port);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            //for now, host on startup?
            Host();
        }

        public async Task Host()
        {
            listener.Start();

            while (true)
            {
                if(currGame == null)
                {
                    currGame = Instantiate(GamePrefab).GetComponent<ServerNetworkController>();
                }
                var client = await listener.AcceptTcpClientAsync();
                currGame.Connect(client);
            }
        }
    }
}
