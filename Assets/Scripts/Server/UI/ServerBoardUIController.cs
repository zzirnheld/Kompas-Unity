using KompasCore.Cards;
using KompasCore.GameCore;
using KompasCore.UI;
using KompasServer.GameCore;

namespace KompasServer.UI
{
	public class ServerBoardUIController : BoardUIController
	{
		public ServerBoardController boardController;
		public override BoardController BoardController => boardController;

		public override UIController UIController => boardController.Game.UIController;

		public override bool InstantiateCues => false;

		public override void Clicked(Space position, GameCard focusCardOverride = null) { }
	}
}