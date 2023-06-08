using KompasCore.Cards.Movement;
using KompasCore.Networking;
using KompasServer.GameCore;
using System.Threading.Tasks;

namespace KompasCore.Networking
{
	public class DebugRehandPacket : Packet
	{
		public int cardId;

		public DebugRehandPacket() : base(DebugRehand) { }

		public DebugRehandPacket(int cardId) : this()
		{
			this.cardId = cardId;
		}

		public override Packet Copy() => new DebugRehandPacket(cardId);
	}
}

namespace KompasServer.Networking
{
	public class DebugRehandServerPacket : DebugRehandPacket, IServerOrderPacket
	{
		public Task Execute(ServerGame serverGame, ServerPlayer player, ServerAwaiter awaiter)
		{
			var card = serverGame.GetCardWithID(cardId);
			if (card == null)
				return Task.CompletedTask;
			else if (serverGame.UIController.DebugMode)
			{
				UnityEngine.Debug.LogWarning($"Debug rehanding card with id {cardId}");
				card.Rehand();
			}
			else
			{
				UnityEngine.Debug.LogError($"Tried to debug rehand card with id {cardId} while NOT in debug mode!");
				player.notifier.NotifyPutBack();
			}
			return Task.CompletedTask;
		}
	}
}