using KompasCore.Networking;
using KompasClient.GameCore;
using KompasServer.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;

namespace KompasCore.Networking
{
    public class GetEffectOptionPacket : Packet
    {
        public string cardName;
        public string choiceBlurb;
        public string[] optionBlurbs;

        public GetEffectOptionPacket() : base(GetEffectOption) { }

        public GetEffectOptionPacket(string cardName, string choiceBlurb, string[] optionBlurbs) : this()
        {
            this.cardName = cardName;
            this.choiceBlurb = choiceBlurb;
            this.optionBlurbs = optionBlurbs;
        }

        public override Packet Copy() => new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs);
    }
}

namespace KompasClient.Networking
{
    public class GetEffectOptionClientPacket : GetEffectOptionPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.clientUICtrl.ShowEffectOptions(choiceBlurb, optionBlurbs);
        }
    }
}