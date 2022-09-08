using KompasCore.Cards;

namespace KompasServer.Cards
{
    public class ServerCardController : CardController
    {
        public ServerGameCard serverCard;

        public override GameCard Card => serverCard;
    }
}