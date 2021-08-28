using KompasClient.GameCore;
using KompasCore.Networking;

namespace KompasCore.Networking
{
    public class EffectImpossiblePacket : Packet
    {
        public EffectImpossiblePacket() : base(EffectImpossible) { }

        public override Packet Copy() => new EffectImpossiblePacket();
    }
}

namespace KompasClient.Networking
{
    public class EffectImpossibleClientPacket : EffectImpossiblePacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame) => clientGame.clientUICtrl.SetCurrState("Effect Impossible");
    }
}