using KompasClient.UI;
using KompasCore.GameCore;

namespace KompasClient.GameCore
{
	public class ClientPlayer : Player
	{
		public ClientPlayer enemy;
		public ClientGame game;

		public ClientPipsUIController pipsUICtrl;

		public ClientDeckController deckController;
		public ClientHandController handController;

		public override Player Enemy => enemy;
		public override bool Friendly => index == 0;
		public override Game Game => game;

		public override int Pips
		{
			get => base.Pips;
			set
			{
				base.Pips = value;
				pipsUICtrl.Pips = value;
			}
		}
	}
}