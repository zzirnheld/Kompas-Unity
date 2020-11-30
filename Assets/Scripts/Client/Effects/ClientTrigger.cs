using KompasCore.Cards;
using KompasCore.Effects;
using UnityEngine;

namespace KompasClient.Effects
{
    public class ClientTrigger : Trigger
    {
        public ClientEffect ClientEffect { get; private set; }

        public override GameCard Source => ClientEffect.Source;
        public override Effect Effect => ClientEffect;

        public ClientTrigger (TriggerData triggerData, ClientEffect parent) : base (triggerData, parent.Game)
        {
            ClientEffect = parent;
        }
    }
}