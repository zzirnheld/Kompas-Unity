﻿using KompasCore.Networking;
using KompasClient.GameCore;

namespace KompasCore.Networking
{
	public class GameEndPacket : Packet
	{
		public bool victory;

		public GameEndPacket() : base(GameEnd) { }

		public GameEndPacket(bool victory, bool invert = false) : this()
		{
			this.victory = (victory && !invert) || (!victory && invert);
		}

		public override Packet Copy() => new GameEndPacket(victory, invert: false);

		public override Packet GetInversion(bool known) => new GameEndPacket(victory, invert: true);
	}
}

namespace KompasClient.Networking
{
	public class GameEndClientPacket : GameEndPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.GameEnded(victory);
		}
	}
}
