using KompasClient.GameCore;

namespace KompasClient.Cards
{
	public class DummyClientGameCard : ClientGameCard
	{
		public void SetClientGame(ClientGame game)
		{
			ClientGame = game;
		}

		public DummyClientGameCard(ClientPlayer owner, ClientCardController clientCardController)
			: base(-1, owner, clientCardController)
		{

		}
	}
}