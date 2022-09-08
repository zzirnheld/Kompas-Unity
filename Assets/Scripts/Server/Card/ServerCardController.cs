using KompasCore.Cards;
using UnityEngine;

namespace KompasServer.Cards
{
    [RequireComponent(typeof(ServerGameCard))]
    public class ServerCardController : CardController
    {
        public ServerGameCard serverCard;

        public override GameCard Card => serverCard;
    }
}