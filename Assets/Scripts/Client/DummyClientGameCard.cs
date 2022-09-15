using KompasClient.GameCore;

namespace KompasClient.Cards
{
    public class DummyClientGameCard : ClientGameCard
    {
        public void SetClientGame(ClientGame game)
        {
            ClientGame = game;
        }

        public DummyClientGameCard(ClientCardController clientCardController)
            : base(clientCardController)
        {

        }
    }
}