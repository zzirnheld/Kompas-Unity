using KompasCore.Networking;
using KompasClient.GameCore;
using KompasClient.UI;

namespace KompasCore.Networking
{
	public class GetEffectOptionPacket : Packet
	{
		public string cardName;
		public string choiceBlurb;
		public string[] optionBlurbs;
		public bool hasDefault;
		public bool showX;
		public int x;

		public GetEffectOptionPacket() : base(GetEffectOption) { }

		public GetEffectOptionPacket(string cardName, string choiceBlurb, string[] optionBlurbs, bool hasDefault, bool showX, int x) : this()
		{
			this.cardName = cardName;
			this.choiceBlurb = choiceBlurb;
			this.optionBlurbs = optionBlurbs;
			this.hasDefault = hasDefault;
			this.x = x;
			this.showX = showX;
		}

		public override Packet Copy() => new GetEffectOptionPacket(cardName, choiceBlurb, optionBlurbs, hasDefault, showX, x);
	}
}

namespace KompasClient.Networking
{
	public class GetEffectOptionClientPacket : GetEffectOptionPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			if (hasDefault && clientGame.clientUIController.OptionalEffAutoResponse == ClientUIController.OptionalEffYes)
				clientGame.clientNotifier.RequestChooseEffectOption(0);
			else if (hasDefault && clientGame.clientUIController.OptionalEffAutoResponse == ClientUIController.OptionalEffNo)
				clientGame.clientNotifier.RequestChooseEffectOption(1);
			else clientGame.clientUIController.ShowEffectOptions(choiceBlurb, optionBlurbs, showX, x);
		}
	}
}