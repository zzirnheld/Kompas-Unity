using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.Effects.Restrictions;
using KompasCore.GameCore;
using KompasCore.Networking;
using Newtonsoft.Json;
using UnityEngine;

namespace KompasCore.Networking
{
	public class GetHandSizeChoicesOrderPacket : Packet
	{
		public int[] cardIds;
		public string listRestrictionJson;

		public GetHandSizeChoicesOrderPacket() : base(ChooseHandSize) { }

		public GetHandSizeChoicesOrderPacket(int[] cardIds, string listRestrictionJson) : this()
		{
			this.cardIds = cardIds;
			this.listRestrictionJson = listRestrictionJson;
		}

		public override Packet Copy() => new GetHandSizeChoicesOrderPacket(cardIds, listRestrictionJson);
	}
}

namespace KompasClient.Networking
{
	public class GetHandSizeChoicesClientPacket : GetHandSizeChoicesOrderPacket, IClientOrderPacket
	{
		public void Execute(ClientGame clientGame)
		{
			clientGame.clientUIController.TargetMode = TargetMode.HandSize;
			IListRestriction listRestriction = null;

			try
			{
				if (listRestrictionJson != null)
					listRestriction = JsonConvert.DeserializeObject<IListRestriction>(listRestrictionJson);
			}
			catch (System.ArgumentException)
			{
				Debug.LogError($"Error loading list restriction from json: {listRestrictionJson}");
			}

			clientGame.SetPotentialTargets(cardIds, listRestriction);
			//TODO make the blurb plural if asking for multiple targets
			clientGame.clientUIController.currentStateUIController.ShuffleToHandSize();
		}
	}
}