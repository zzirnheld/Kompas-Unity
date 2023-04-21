using KompasClient.GameCore;
using KompasCore.Networking;
using System.Linq;

namespace KompasCore.Networking
{
	public class RemoveTargetPacket : Packet
	{
		public int sourceCardId;
		public int effIndex;
		public int targetCardId;

		public RemoveTargetPacket() : base(RemoveTarget) { }

		public RemoveTargetPacket(int sourceCardId, int effIndex, int targetCardId) : this()
		{
			this.sourceCardId = sourceCardId;
			this.effIndex = effIndex;
			this.targetCardId = targetCardId;
		}

		public override Packet Copy() => new RemoveTargetPacket(sourceCardId, effIndex, targetCardId);
	}
}

namespace KompasClient.Networking
{
	public class RemoveTargetClientPacket : RemoveTargetPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			var source = clientGame.GetCardWithID(sourceCardId);
			var target = clientGame.GetCardWithID(targetCardId);
			if (source != null && target != null) source.Effects.ElementAt(effIndex)?.RemoveTarget(target);
		}
	}
}