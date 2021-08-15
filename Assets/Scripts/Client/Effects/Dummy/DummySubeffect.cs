using UnityEngine;
using KompasCore.Effects;
using KompasCore.GameCore;
using KompasClient.GameCore;
using Newtonsoft.Json;

namespace KompasClient.Effects
{
    public class DummySubeffect : Subeffect
    {
        public override Effect Effect => ClientEffect;
        public override Player Controller => ClientController;
        public override Game Game => ClientEffect.Game;

        public ClientEffect ClientEffect { get; private set; }
        public ClientPlayer ClientController => ClientEffect.ClientController;

        public static DummySubeffect FromJson(string json, ClientEffect parent, int subeffIndex)
        {
            var subeff = JsonConvert.DeserializeObject<Subeffect>(json);

            Debug.Log($"Creating subeffect from json {json}");
            DummySubeffect toReturn;

            toReturn = new DummySubeffect();

            if (toReturn != null)
            {
                Debug.Log($"Finishing setup for new effect of type {subeff.subeffType}");
                toReturn.Initialize(parent, subeffIndex);
            }

            return toReturn;
        }

        public virtual void Initialize(ClientEffect eff, int subeffIndex)
        {
            this.ClientEffect = eff;
            this.SubeffIndex = subeffIndex;
        }
    }
}