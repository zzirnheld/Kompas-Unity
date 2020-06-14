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
        
        private TcpListener listener;
        private List<ServerGame> games;
        private ServerGame currGame = null;

        private void Awake()
        {
            games = new List<ServerGame>();
            try
            {
                listener = new TcpListener(IPAddress.Any, NetworkController.port);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
            }

            //host on startup
            //Don't await, so execution continues instead of hanging waiting for connection
            Host();
        }

        public async void Host()
        {
            listener.Start();

            while (true)
            {
                if(currGame == null)
                {
                    currGame = Instantiate(GamePrefab).GetComponent<ServerGame>();
                    currGame.Init(UICtrl, CardRepo);
                    games.Add(currGame);
                }
                Debug.Log("Waiting for next client");
                var client = listener.AcceptTcpClient();
                Debug.Log("Connected to a client");
                if(currGame.AddPlayer(client) >= 2) currGame = null;
            }
        }
    }
}
