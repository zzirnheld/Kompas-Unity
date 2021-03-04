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
        public bool hasDefault;

        public GetEffectOptionPacket() : base(GetEffectOption) { }

        public GetEffectOptionPacket(string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault) : this()
        {
            this.cardName = cardName;
            this.choiceBlurb = choiceBlurb;
            this.optionBlurbs = optionBlurbs;
            this.hasDefault = hasDefault;
        }

        public override Packet Copy() => new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs, hasDefault);
    }
}

namespace KompasClient.Networking
{
    public class GetEffectOptionClientPacket : GetEffectOptionPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            if (hasDefault && clientGame.clientUICtrl.AutoYesOptional) clientGame.clientNotifier.RequestChooseEffectOption(0);
            else clientGame.clientUICtrl.ShowEffectOptions(choiceBlurb, optionBlurbs);
        }
    }
}