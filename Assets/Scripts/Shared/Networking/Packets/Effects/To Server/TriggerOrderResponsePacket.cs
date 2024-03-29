﻿using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
	public class TriggerOrderResponsePacket : Packet
	{
		public int[] cardIds;
		public int[] effIndices;
		public int[] orders;

		public TriggerOrderResponsePacket() : base(ChooseTriggerOrder) { }

		public TriggerOrderResponsePacket(int[] cardIds, int[] effIndices, int[] orders) : this()
		{
			this.cardIds = cardIds;
			this.effIndices = effIndices;
			this.orders = orders;
		}

		public override Packet Copy() => new TriggerOrderResponsePacket(cardIds, effIndices, orders);
	}
}

namespace KompasServer.Networking
{
	public class TriggerOrderResponseServerPacket : TriggerOrderResponsePacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			awaiter.TriggerOrders = (cardIds, effIndices, orders);
			return Task.CompletedTask;
		}
	}
}