using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class ToggleDecliningTargetPacket : Packet
    {
        public bool enabled;

        public ToggleDecliningTargetPacket() : base(ToggleDecliningTarget) { }

        public ToggleDecliningTargetPacket(bool enabled) : this()
        {
            this.enabled = enabled;
        }

        public override Packet Copy() => new ToggleDecliningTargetPacket(enabled);
    }
}

namespace KompasClient.Networking
{
    public class ToggleDecliningTargetClientPacket : ToggleDecliningTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            if (enabled) clientGame.clientUICtrl.EnableDecliningTarget();
            else clientGame.clientUICtrl.DisableDecliningTarget();
        }
    }
}