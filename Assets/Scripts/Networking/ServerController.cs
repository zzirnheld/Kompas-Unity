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
        public UIController UICtrl;
        public CardRepository CardRepo;

        private IPAddress ipAddress;
        private TcpListener listener;
        private List<ServerGame> games;
        private ServerGame currGame = null;
        private int numClients = 0;

        private void Awake()
        {
            ipAddress = Dns.GetHostEntry("localhost").AddressList[0];

            games = new List<ServerGame>();
            try
            {
                listener = new TcpListener(ipAddress, NetworkController.port);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            //for now, host on startup?
            //Don't await, so execution continues instead of hanging waiting for connection
            Host();
        }

        public async Task Host()
        {
            Debug.Log($"Hosting on {ipAddress.ToString()}");
            listener.Start();

            while (true)
            {
                if(currGame == null)
                {
                    currGame = Instantiate(GamePrefab).GetComponent<ServerGame>();
                    currGame.Init(UICtrl, CardRepo);
                    games.Add(currGame);
                }
                var client = await listener.AcceptTcpClientAsync();
                if(currGame.AddPlayer(client) >= 2) currGame = null;
            }
        }
    }
}
