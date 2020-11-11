namespace KompasClient.GameCore
{
    public class ClientPlayer : Player
    {
        public ClientPlayer ClientEnemy;
        public ClientGame clientGame;

        public override Player Enemy => ClientEnemy;

        public override int Pips 
        { 
            get => base.Pips;
            set
            {
                base.Pips = value;
                if (index == 0) clientGame.clientUICtrl.FriendlyPips = Pips; 
                else clientGame.clientUICtrl.EnemyPips = Pips;
            }
        }
    }
}