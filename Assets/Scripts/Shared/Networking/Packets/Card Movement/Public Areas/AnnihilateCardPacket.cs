using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Cards.Movement;

namespace KompasCore.Networking
{
	public class AnnihilateCardPacket : Packet
	{
		public int cardId;
		public string json;
		public int controllerIndex;

		public AnnihilateCardPacket() : base(AnnihilateCard) { }

		public AnnihilateCardPacket(int cardId, string json, int controllerIndex, bool invert = false) : this()
		{
			this.cardId = cardId;
			this.json = json;
			this.controllerIndex = invert ? 1 - controllerIndex : controllerIndex;
		}

		public override Packet Copy() => new AnnihilateCardPacket(cardId, json, controllerIndex, invert: false);

		public override Packet GetInversion(bool known)
		{
			if (known) return new AnnihilateCardPacket(cardId, json, controllerIndex, invert: true);
			else return new AddCardPacket(cardId, json, CardLocation.Annihilation, controllerIndex, nowKnown: true, invert: true);
		}
	}
}

namespace KompasClient.Networking
{
	public class AnnihilateCardClientPacket : AnnihilateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
			=> clientGame.GetCardWithID(cardId)?.Annihilate();
	}
}