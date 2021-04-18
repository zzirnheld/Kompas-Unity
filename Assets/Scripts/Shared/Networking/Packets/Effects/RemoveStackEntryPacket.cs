using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class RemoveStackEntryPacket : Packet
    {
        public int indexToRemove;

        public RemoveStackEntryPacket() : base(RemoveStackEntry) { }

        public RemoveStackEntryPacket(int indexToRemove) : this()
        {
            this.indexToRemove = indexToRemove;
        }

        public override Packet Copy() => new RemoveStackEntryPacket(indexToRemove);
    }
}

namespace KompasClient.Networking
{
    public class RemoveStackEntryClientPacket : RemoveStackEntryPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.clientEffectsCtrl.Remove(indexToRemove);
    }
}