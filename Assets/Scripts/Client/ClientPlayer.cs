using KompasClient.UI;

namespace KompasClient.GameCore
{
    public class ClientPlayer : Player
    {
        public ClientPlayer ClientEnemy;
        public ClientGame clientGame;

        public ClientPipsUIController pipsUICtrl;

        public ClientDeckController clientDeckCtrl;
        public ClientHandController clientHandCtrl;

        public override Player Enemy => ClientEnemy;
        public override bool Friendly => index == 0;

        public override int Pips
        {
            get => base.Pips;
            set
            {
                base.Pips = value;
                if (index == 0) clientGame.clientUIController.FriendlyPips = Pips;
                else clientGame.clientUIController.EnemyPips = Pips;
                pipsUICtrl.Pips = value;
            }
        }
    }
}