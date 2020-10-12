using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class SetAvatarPacket : Packet
    {
        public int playerIndex;
        public string cardName;
        public int cardId;

        public SetAvatarPacket() : base(SetAvatar) { }

        public SetAvatarPacket(int playerIndex, string cardName, int cardId) : this()
        {
            this.playerIndex = playerIndex;
            this.cardName = cardName;
            this.cardId = cardId;
        }

        public override Packet Copy() => new SetAvatarPacket();

        public override Packet GetInversion(bool known = true) 
            => new SetAvatarPacket(1 - playerIndex, cardName, cardId);
    }
}

namespace KompasClient.Networking
{
    public class SetAvatarClientPacket : SetAvatarPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.SetAvatar(playerIndex, cardName, cardId);
    }
}