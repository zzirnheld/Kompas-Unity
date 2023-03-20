using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
    public class ChangeEnemyHandCountPacket : Packet
    {
        public int num;

        public ChangeEnemyHandCountPacket() : base(ChangeEnemyHandCount) { }

        public ChangeEnemyHandCountPacket(int num) : this()
        {
            this.num = num;
        }

        public override Packet Copy() => new ChangeEnemyHandCountPacket(num);
    }
}

namespace KompasClient.Networking
{
    public class ChangeEnemyHandCountClientPacket : ChangeEnemyHandCountPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            for (int i = 0; i < num; i++) clientGame.clientPlayers[1].handController.IncrementHand();
            for (int i = 0; i > num; i--) clientGame.clientPlayers[1].handController.DecrementHand();
        }
    }
}