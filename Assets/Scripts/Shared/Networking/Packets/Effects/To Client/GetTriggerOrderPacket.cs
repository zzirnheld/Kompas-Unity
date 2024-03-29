﻿using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.Networking;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Networking
{
	public class GetTriggerOrderPacket : Packet
	{
		public int[] sourceCardIds;
		public int[] effIndices;

		public GetTriggerOrderPacket() : base(GetTriggerOrder) { }

		public GetTriggerOrderPacket(int[] sourceCardIds, int[] effIndices) : this()
		{
			this.sourceCardIds = sourceCardIds;
			this.effIndices = effIndices;
		}

		public override Packet Copy() => new GetTriggerOrderPacket(sourceCardIds, effIndices);
	}
}

namespace KompasClient.Networking
{
	public class GetTriggerOrderClientPacket : GetTriggerOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			List<Trigger> triggers = new List<Trigger>();
			for (int i = 0; i < sourceCardIds.Length; i++)
			{
				var card = clientGame.GetCardWithID(sourceCardIds[i]);
				var trigger = card?.Effects.ElementAt(effIndices[i]).Trigger;
				if (trigger != null) triggers.Add(trigger);
			}
			clientGame.clientUIController.triggerOrderUI.OrderTriggers(triggers);
		}
	}
}