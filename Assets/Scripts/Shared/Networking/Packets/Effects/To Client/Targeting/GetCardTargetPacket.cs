using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using System.Linq;
using KompasClient.Effects;
using UnityEngine;

namespace KompasCore.Networking
{
    public class GetCardTargetPacket : Packet
    {
        public string sourceCardName;
        public string targetBlurb;
        public int[] potentialTargetIds;
        public string listRestrictionJson;

        public GetCardTargetPacket() : base(GetCardTarget) { }

        public GetCardTargetPacket(string sourceCardName, string targetBlurb, int[] potentialTargetIds, string listRestrictionJson) : this()
        {
            this.sourceCardName = sourceCardName;
            this.targetBlurb = targetBlurb;
            this.potentialTargetIds = potentialTargetIds;
            this.listRestrictionJson = listRestrictionJson;
        }

        public override Packet Copy() => new GetCardTargetPacket(sourceCardName, targetBlurb, potentialTargetIds, listRestrictionJson);
    }
}

namespace KompasClient.Networking
{
    public class GetCardTargetClientPacket : GetCardTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = Game.TargetMode.CardTarget;
            ListRestriction listRestriction = null;

            try
            {
                if(listRestrictionJson != null)
                    listRestriction = JsonUtility.FromJson<ListRestriction>(listRestrictionJson);
            }
            catch(System.ArgumentException)
            {
                Debug.LogError($"Error loading list restriction from json: {listRestrictionJson}");
            }

            clientGame.SetPotentialTargets(potentialTargetIds, listRestriction);
            //TODO make the blurb plural if asking for multiple targets
            clientGame.clientUICtrl.SetCurrState($"Choose {sourceCardName}'s Card Target", targetBlurb);
        }
    }
}