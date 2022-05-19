using KompasCore.Networking;
using KompasClient.GameCore;
using KompasCore.Effects;
using KompasCore.GameCore;
using UnityEngine;
using Newtonsoft.Json;

namespace KompasCore.Networking
{
    public class GetCardTargetPacket : Packet
    {
        public string sourceCardName;
        public string targetBlurb;
        public int[] potentialTargetIds;
        public string listRestrictionJson;
        public bool list;

        public GetCardTargetPacket() : base(GetCardTarget) { }

        public GetCardTargetPacket(string sourceCardName, string targetBlurb, int[] potentialTargetIds, string listRestrictionJson, bool list) : this()
        {
            this.sourceCardName = sourceCardName;
            this.targetBlurb = targetBlurb;
            this.potentialTargetIds = potentialTargetIds;
            this.listRestrictionJson = listRestrictionJson;
            this.list = list;
        }

        public override Packet Copy() => new GetCardTargetPacket(sourceCardName, targetBlurb, potentialTargetIds, listRestrictionJson, list);
    }
}

namespace KompasClient.Networking
{
    public class GetCardTargetClientPacket : GetCardTargetPacket, IClientOrderPacket
    {
        public void Execute(ClientGame clientGame)
        {
            clientGame.targetMode = list ? Game.TargetMode.CardTargetList : Game.TargetMode.CardTarget;
            ListRestriction listRestriction = null;

            try
            {
                if (string.IsNullOrEmpty(listRestrictionJson)) listRestriction = ListRestriction.Default;
                else listRestriction = JsonConvert.DeserializeObject<ListRestriction>(listRestrictionJson);

                listRestriction.Initialize(new EffectInitializationContext(game: clientGame, source: default));
            }
            catch (System.ArgumentException)
            {
                Debug.LogError($"Error loading list restriction from json: {listRestrictionJson}");
            }

            clientGame.SetPotentialTargets(potentialTargetIds, listRestriction);
            //TODO make the blurb plural if asking for multiple targets
            clientGame.clientUICtrl.SetCurrState($"Choose {sourceCardName}'s Card Target", targetBlurb);
        }
    }
}