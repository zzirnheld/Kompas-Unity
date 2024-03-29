﻿using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
	public class NegateCardPacket : Packet
	{
		public int cardId;
		public bool negated;

		public NegateCardPacket() : base(NegateCard) { }

		public NegateCardPacket(int cardId, bool negated) : this()
		{
			this.cardId = cardId;
			this.negated = negated;
		}

		public override Packet Copy() => new NegateCardPacket(cardId, negated);
	}
}

namespace KompasClient.Networking
{
	public class NegateCardClientPacket : NegateCardPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var card = clientGame.GetCardWithID(cardId);
			if (card != null) card.SetNegated(negated);
		}
	}
}