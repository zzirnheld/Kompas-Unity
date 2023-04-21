using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
	public class ActivateCardPacket : Packet
	{
		public int cardId;
		public bool activated;

		public ActivateCardPacket() : base(ActivateCard) { }

		public ActivateCardPacket(int cardId, bool activated) : this()
		{
			this.cardId = cardId;
			this.activated = activated;
		}

		public override Packet Copy() => new ActivateCardPacket(cardId, activated);
	}
}

namespace KompasClient.Networking
{
	public class ActivateCardClientPacket : ActivateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.GetCardWithID(cardId);
			if (card != null) card.SetActivated(activated);
		}
	}
}