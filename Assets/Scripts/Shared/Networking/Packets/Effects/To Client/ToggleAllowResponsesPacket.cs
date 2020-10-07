using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class ToggleAllowResponsesPacket : Packet
    {
        public bool enabled;

        public ToggleAllowResponsesPacket() : base(ToggleAllowResponses) { }

        public ToggleAllowResponsesPacket(bool enabled) : this()
        {
            this.enabled = enabled;
        }

        public override Packet Copy() => new ToggleAllowResponsesPacket(enabled);
    }
}

namespace KompasClient.Networking
{
    public class ToggleAllowResponsesClientPacket : ToggleAllowResponsesPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            if (enabled) clientGame.clientUICtrl.GetResponse();
            else clientGame.clientUICtrl.UngetResponse();
        }
    }
}